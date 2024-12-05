using Hexa.NET.ImGui;
using System.Numerics;
using System.Text.RegularExpressions;

namespace MiscTools.Services.Ui;

internal sealed class FileSystemRenameWindow : WindowBase
{
	private string _directory = string.Empty;
	private string _fileFilter = "*.png";
	private SearchOption _searchOption = SearchOption.TopDirectoryOnly;

	private string _pattern = @"(.*)\.png";
	private string _replacement = "$1.jpg";

	private ActionPreview? _preview;

	public override void Render()
	{
		if (ImGui.Begin("File System Rename"))
		{
			ImGui.Text("Rename files in a directory.");
			ImGui.Separator();

			RenderSettingsTable();

			if (ImGui.Button("Preview"))
				Preview();

			if (_preview != null)
			{
				ImGui.Text($"Total file count: {_preview.TotalFileCount}");
				ImGui.Text($"Total filter matches: {_preview.TotalFilterMatches}");
				ImGui.Text($"Total regex matches: {_preview.TotalRegexMatches}");

				if (ImGui.BeginTable("PreviewTable", 2, ImGuiTableFlags.ScrollY, new Vector2(0, 320)))
				{
					ImGui.TableSetupColumn("Old file path", ImGuiTableColumnFlags.WidthStretch);
					ImGui.TableSetupColumn("New file path", ImGuiTableColumnFlags.WidthStretch);
					ImGui.TableSetupScrollFreeze(0, 1);
					ImGui.TableHeadersRow();

					foreach ((string oldName, string newName) in _preview.Files)
					{
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						ImGui.Text(oldName);
						ImGui.TableNextColumn();
						ImGui.Text(newName);
					}

					ImGui.EndTable();
				}

				if (ImGui.Button("Rename"))
					Rename();
			}
		}

		ImGui.End();
	}

	private void RenderSettingsTable()
	{
		if (!ImGui.BeginTable("SettingsTable", 2))
			return;

		ImGui.TableSetupColumn("Setting", ImGuiTableColumnFlags.WidthFixed, 100);
		ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);

		SetUpSetting("Directory");
		ImGui.InputText("##Directory", ref _directory, 1024);

		SetUpSetting("File filter");
		ImGui.InputText("##FileFilter", ref _fileFilter, 1024);

		SetUpSetting("Search option");
		int searchOptionInt = (int)_searchOption;
		if (ImGui.Combo("##SearchOption", ref searchOptionInt, "Top directory only\0All directories", 2))
			_searchOption = (SearchOption)searchOptionInt;

		SetUpSetting("Pattern");
		ImGui.InputText("##Pattern", ref _pattern, 1024);

		SetUpSetting("Replacement");
		ImGui.InputText("##Replacement", ref _replacement, 1024);

		ImGui.EndTable();

		static void SetUpSetting(string label)
		{
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.Text(label);
			ImGui.TableNextColumn();
		}
	}

	private void Preview()
	{
		if (string.IsNullOrWhiteSpace(_directory) || string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(_fileFilter))
			return;

		DirectoryInfo directoryInfo = new(_directory);
		string[] matchedFiles = GetFiles(directoryInfo);
		Dictionary<string, string> previewFiles = matchedFiles
			.Select(oldFilePath =>
			{
				string oldFileName = Path.GetFileName(oldFilePath);
				string newFileName = GetRenamedFileName(oldFileName);
				return (oldFilePath, newFilePath: Path.Combine(_directory, newFileName));
			})
			.ToDictionary(t => t.oldFilePath, t => t.newFilePath);

		int totalFileCount = directoryInfo.GetFiles(_fileFilter, _searchOption).Length;
		int totalFilterMatches = matchedFiles.Length;
		_preview = new ActionPreview(totalFileCount, totalFilterMatches, previewFiles.Count, previewFiles);
	}

	private string[] GetFiles(DirectoryInfo directory)
	{
		if (!directory.Exists)
			return [];

		return Directory.GetFiles(_directory, _fileFilter, _searchOption);
	}

	private void Rename()
	{
		if (string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(_fileFilter))
			return;

		DirectoryInfo directoryInfo = new(_directory);
		foreach (string oldFilePath in GetFiles(directoryInfo))
		{
			string oldFileName = Path.GetFileName(oldFilePath);
			string newFileName = GetRenamedFileName(oldFileName);
			File.Move(oldFilePath, Path.Combine(_directory, newFileName));
		}
	}

	private string GetRenamedFileName(string fileName)
	{
		return Regex.Replace(fileName, _pattern, _replacement);
	}

	private sealed record ActionPreview(int TotalFileCount, int TotalFilterMatches, int TotalRegexMatches, Dictionary<string, string> Files);
}
