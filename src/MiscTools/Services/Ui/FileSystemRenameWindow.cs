using Hexa.NET.ImGui;
using System.Text.RegularExpressions;

namespace MiscTools.Services.Ui;

internal sealed class FileSystemRenameWindow : WindowBase
{
	private string _directory = string.Empty;
	private string _fileFilter = "*.png";
	private string _pattern = @"(.*)\.png";
	private string _replacement = "$1.jpg";

	private string[] _preview = [];

	public override void Render()
	{
		if (ImGui.Begin("File System Rename"))
		{
			ImGui.Text("Rename files in a directory.");
			ImGui.Separator();

			if (ImGui.BeginTable("SettingsTable", 2))
			{
				ImGui.TableSetupColumn("Setting", ImGuiTableColumnFlags.WidthFixed, 100);
				ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text("Directory");
				ImGui.TableNextColumn();
				ImGui.InputText("##Directory", ref _directory, 1024);

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text("File filter");
				ImGui.TableNextColumn();
				ImGui.InputText("##FileFilter", ref _fileFilter, 1024);

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text("Pattern");
				ImGui.TableNextColumn();
				ImGui.InputText("##Pattern", ref _pattern, 1024);

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text("Replacement");
				ImGui.TableNextColumn();
				ImGui.InputText("##Replacement", ref _replacement, 1024);

				ImGui.EndTable();
			}

			if (ImGui.Button("Preview"))
				Preview();

			foreach (string line in _preview)
				ImGui.Text(line);

			if (ImGui.Button("Rename"))
				Rename();
		}

		ImGui.End();
	}

	private void Preview()
	{
		if (string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(_fileFilter))
			return;

		DirectoryInfo directory = new(_directory);
		if (!directory.Exists)
			return;

		_preview = Directory.GetFiles(_directory, _fileFilter)
			.Select(file =>
			{
				string fileName = Path.GetFileName(file);
				string newFileName = Regex.Replace(fileName, _pattern, _replacement);
				return $"{fileName} -> {newFileName}";
			})
			.ToArray();
	}

	private void Rename()
	{
		if (string.IsNullOrWhiteSpace(_pattern) || string.IsNullOrWhiteSpace(_fileFilter))
			return;

		DirectoryInfo directory = new(_directory);
		if (!directory.Exists)
			return;

		foreach (string file in Directory.GetFiles(_directory, _fileFilter))
		{
			string fileName = Path.GetFileName(file);
			string newFileName = Regex.Replace(fileName, _pattern, _replacement);
			string newFilePath = Path.Combine(_directory, newFileName);
			File.Move(file, newFilePath);
		}
	}
}
