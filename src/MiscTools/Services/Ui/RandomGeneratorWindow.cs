using Detach;
using ImGuiNET;

namespace MiscTools.Services.Ui;

public sealed class RandomGeneratorWindow : WindowBase
{
	private Guid _randomGuid = Guid.NewGuid();

	public override void Render()
	{
		if (ImGui.Begin("Random Generator"))
		{
			RandomGuid();
		}

		ImGui.End();
	}

	private void RandomGuid()
	{
		if (ImGui.Button("Random GUID"))
			_randomGuid = Guid.NewGuid();

		ImGui.SameLine();
		ImGui.Text(Inline.Span(_randomGuid));

		ImGui.SameLine();
		if (ImGui.Button("Copy"))
			ImGui.SetClipboardText(_randomGuid.ToString());
	}
}
