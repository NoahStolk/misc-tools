using Detach.Numerics;
using Hexa.NET.ImGui;
using System.Numerics;

namespace MiscTools.Services.Ui;

public sealed class InputDebugWindow : WindowBase
{
	private static readonly string[] _debugTextInput =
	[
		"Type letters and numbers: ",
		"NOW HOLD SHIFT: ",
		"Enter some enters, and use the arrow keys to navigate.\nUse backspace and delete to remove text.",
		"Insert some tabs (only works for this input field).\nHold keys to see the repeat rate.",
		"Select all text, copy, paste, and use CTRL + arrows to navigate between words.\nUse CTRL + backspace to delete words.",
		"Use SHIFT + arrows / home / end / page up / page down to select text.",
	];
	private bool _checkbox;

	public override void Render()
	{
		if (ImGui.Begin("Input debug"))
		{
			ImGui.SeparatorText("Test keyboard input");

			ImGui.InputText("Letters, numbers", ref _debugTextInput[0], 1024);
			ImGui.InputText("Letters, numbers (SHIFT)", ref _debugTextInput[1], 1024);

			ImGui.InputTextMultiline("Enter, arrow keys, backspace, delete", ref _debugTextInput[2], 1024, new Vector2(0, 64));
			ImGui.InputTextMultiline("Tab", ref _debugTextInput[3], 1024, new Vector2(0, 64), ImGuiInputTextFlags.AllowTabInput);
			ImGui.InputTextMultiline("CTRL shortcut\n- CTRL+A\n- CTRL+C\n- CTRL+V\n- CTRL+arrows", ref _debugTextInput[4], 1024, new Vector2(0, 64));
			ImGui.InputTextMultiline("SHIFT shortcuts\n- SHIFT+arrows\n- SHIFT+home", ref _debugTextInput[5], 1024, new Vector2(0, 64));

			ImGui.SeparatorText("Test mouse input");

			ImGui.Checkbox("Checkbox", ref _checkbox);

			if (ImGui.BeginChild("Scroll area", new Vector2(256, 128)))
			{
				for (int i = 0; i < 50; i++)
				{
					Rgba color = (i % 3) switch
					{
						0 => Rgba.Yellow,
						1 => Rgba.Aqua,
						_ => Rgba.Red,
					};
					ReadOnlySpan<byte> text = (i % 3) switch
					{
						0 => "Scrolling should not go to top or bottom instantly"u8,
						1 => "Scrolling should go evenly per frame (not missing inputs or jumping)"u8,
						_ => "This should work with and without VSync"u8,
					};

					ImGui.PushStyleColor(ImGuiCol.Text, color);
					ImGui.TextWrapped(text);
					ImGui.PopStyleColor();

					ImGui.Separator();
				}
			}

			ImGui.EndChild();
		}

		ImGui.End();
	}
}
