using Detach;
using Hexa.NET.ImGui;

namespace MiscTools.Services.Ui;

internal sealed class HtmlChangeDetectionWindow : WindowBase
{
	private bool _enable;
	private float _timer;
	private float _refreshRate = 15;
	private string _url = "https://www.google.com";
	private string? _lastHtml;
	private readonly List<Log> _logs = [];

	public override void Render(in float dt)
	{
		Logic(dt);

		if (ImGui.Begin("HTML Change Detection"))
		{
			ImGui.Text(Inline.Utf8($"{_refreshRate - _timer:0.0}s until next refresh"));
			ImGui.SliderFloat("Refresh rate", ref _refreshRate, 1, 30, "%.0fs");
			ImGui.InputText("URL", ref _url, 256);
			ImGui.Checkbox("Enable", ref _enable);

			ImGui.Separator();

			if (ImGui.Button("Clear logs"))
				_logs.Clear();

			if (ImGui.BeginTable("Logs", 2, ImGuiTableFlags.Borders))
			{
				ImGui.TableSetupColumn("Message", ImGuiTableColumnFlags.WidthStretch);
				ImGui.TableSetupColumn("Timestamp", ImGuiTableColumnFlags.WidthFixed, 160);
				ImGui.TableHeadersRow();

				foreach (Log log in _logs)
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.Text(log.Message);
					ImGui.TableNextColumn();
					ImGui.Text(Inline.Utf8(log.Timestamp, "yyyy-MM-dd HH:mm:ss"));
				}

				ImGui.EndTable();
			}
		}

		ImGui.End();
	}

	private void Logic(in float dt)
	{
		if (!_enable)
			return;

		_timer += dt;
		if (_timer >= _refreshRate)
		{
			_timer -= _refreshRate;
			Task.Run(async () => Callback(await DownloadHtmlAsync()));
		}
	}

	private async Task<string> DownloadHtmlAsync()
	{
		using HttpClient client = new();
		return await client.GetStringAsync(new Uri(_url));
	}

	private void Callback(string html)
	{
		try
		{
			if (html == _lastHtml)
			{
				_logs.Add(new Log("No change", DateTime.Now));
				return;
			}

			File.WriteAllText($"{DateTime.Now.ToFileTime()}.html", html);

			_lastHtml = html;
			_logs.Add(new Log("Change detected", DateTime.Now));
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}

	private sealed record Log(string Message, DateTime Timestamp);
}
