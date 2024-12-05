using Hexa.NET.ImGui;

namespace MiscTools.Services.Ui;

internal sealed class MainWindow(IReadOnlyList<WindowBase> windows)
{
	private string _filter = string.Empty;

	public void Render()
	{
		if (ImGui.Begin("Main Window"))
		{
			ImGui.SeparatorText("Browse Tools");

			ImGui.InputText("Filter", ref _filter, 100);

			if (ImGui.Button("Clear Filter"))
				_filter = string.Empty;

			if (ImGui.Button("Close All"))
			{
				for (int i = 0; i < windows.Count; i++)
					windows[i].IsVisible = false;
			}

			ImGui.Separator();

			for (int i = 0; i < windows.Count; i++)
			{
				if (!windows[i].Title.Contains(_filter, StringComparison.OrdinalIgnoreCase))
					continue;

				WindowBase window = windows[i];
				bool isVisible = window.IsVisible;
				if (ImGui.Checkbox(window.Title, ref isVisible))
					window.IsVisible = !window.IsVisible;
			}
		}

		ImGui.End();

		for (int i = 0; i < windows.Count; i++)
		{
			WindowBase window = windows[i];
			if (window.IsVisible)
				window.Render();
		}
	}
}
