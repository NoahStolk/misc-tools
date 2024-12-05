using Detach;
using Hexa.NET.ImGui;

namespace MiscTools.Services.Ui;

internal sealed class RandomGeneratorWindow : WindowBase
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
		ImGui.Text(Inline.Utf8(_randomGuid));

		ImGui.SameLine();
		if (ImGui.Button("Copy"))
			ImGui.SetClipboardText(_randomGuid.ToString());
	}
}
