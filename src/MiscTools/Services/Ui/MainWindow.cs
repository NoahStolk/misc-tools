using Hexa.NET.ImGui;

namespace MiscTools.Services.Ui;

public sealed class MainWindow
{
	private readonly IReadOnlyList<WindowBase> _windows;

	private string _filter = string.Empty;

	public MainWindow(IReadOnlyList<WindowBase> windows)
	{
		_windows = windows;
	}

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
				for (int i = 0; i < _windows.Count; i++)
					_windows[i].IsVisible = false;
			}

			ImGui.Separator();

			for (int i = 0; i < _windows.Count; i++)
			{
				if (!_windows[i].Title.Contains(_filter, StringComparison.OrdinalIgnoreCase))
					continue;

				WindowBase window = _windows[i];
				bool isVisible = window.IsVisible;
				if (ImGui.Checkbox(window.Title, ref isVisible))
					window.IsVisible = !window.IsVisible;
			}
		}

		ImGui.End();

		for (int i = 0; i < _windows.Count; i++)
		{
			WindowBase window = _windows[i];
			if (window.IsVisible)
				window.Render();
		}
	}
}
