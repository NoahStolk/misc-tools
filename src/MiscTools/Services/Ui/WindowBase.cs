namespace MiscTools.Services.Ui;

internal abstract class WindowBase
{
	protected WindowBase()
	{
		Title = GetType().Name;
	}

	public string Title { get; }

	public bool IsVisible { get; set; }

	public abstract void Render(in float dt);
}
