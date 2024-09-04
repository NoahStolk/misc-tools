namespace MiscTools.Services.Ui;

public abstract class WindowBase
{
	protected WindowBase()
	{
		Title = GetType().Name;
	}

	public string Title { get; }

	public bool IsVisible { get; set; }

	public abstract void Render();
}
