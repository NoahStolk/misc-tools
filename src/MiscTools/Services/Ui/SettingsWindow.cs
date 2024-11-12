using Hexa.NET.ImGui;
using Silk.NET.GLFW;

namespace MiscTools.Services.Ui;

public sealed class SettingsWindow : WindowBase
{
	private readonly Glfw _glfw;

	private bool _vsync;

	public SettingsWindow(Glfw glfw)
	{
		_glfw = glfw;
	}

	public override void Render()
	{
		if (ImGui.Begin("Settings"))
		{
			if (ImGui.Checkbox("VSync", ref _vsync))
				_glfw.SwapInterval(_vsync ? 1 : 0);
		}

		ImGui.End();
	}
}
