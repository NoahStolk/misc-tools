using Hexa.NET.ImGui;
using Silk.NET.GLFW;

namespace MiscTools.Services.Ui;

internal sealed class SettingsWindow(Glfw glfw) : WindowBase
{
	private bool _vsync;

	public override void Render()
	{
		if (ImGui.Begin("Settings"))
		{
			if (ImGui.Checkbox("VSync", ref _vsync))
				glfw.SwapInterval(_vsync ? 1 : 0);
		}

		ImGui.End();
	}
}
