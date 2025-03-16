﻿using Detach.GlfwExtensions;
using Detach.ImGuiBackend.GlfwHexa;
using MiscTools.Extensions;
using MiscTools.Services;
using MiscTools.Services.Ui;
using MiscTools.Utils;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using StrongInject;

namespace MiscTools;

[Register<GlfwInput>(Scope.SingleInstance)]
[Register<App>(Scope.SingleInstance)]

[Register<MainWindow>(Scope.SingleInstance)]

[Register<FileSystemRenameWindow>(Scope.SingleInstance)]
[Register<HtmlChangeDetectionWindow>(Scope.SingleInstance)]
[Register<NetworkWindow>(Scope.SingleInstance)]
[Register<RandomGeneratorWindow>(Scope.SingleInstance)]
#pragma warning disable S3881 // "IDisposable" should be implemented correctly. The source generator already implements IDisposable correctly.
internal sealed partial class Container : IContainer<App>
#pragma warning restore S3881
{
	[Factory(Scope.SingleInstance)]
	private static Glfw GetGlfw()
	{
		Glfw glfw = Glfw.GetApi();
		glfw.Init();
		glfw.CheckError();

		glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
		glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
		glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
		glfw.WindowHint(WindowHintBool.Focused, true);
		glfw.WindowHint(WindowHintBool.Resizable, true);
		glfw.CheckError();

		return glfw;
	}

	[Factory(Scope.SingleInstance)]
	private static unsafe WindowHandle* CreateWindow(Glfw glfw, GlfwInput glfwInput)
	{
		WindowHandle* window = glfw.CreateWindow(WindowConstants.WindowWidth, WindowConstants.WindowHeight, WindowConstants.WindowTitle, null, null);
		glfw.CheckError();
		if (window == null)
			throw new InvalidOperationException("Could not create window.");

		glfw.SetCursorPosCallback(window, (_, x, y) => glfwInput.CursorPosCallback(x, y));
		glfw.SetScrollCallback(window, (_, _, y) => glfwInput.MouseWheelCallback(y));
		glfw.SetMouseButtonCallback(window, (_, button, state, _) => glfwInput.MouseButtonCallback(button, state));
		glfw.SetKeyCallback(window, (_, keys, _, state, _) => glfwInput.KeyCallback(keys, state));
		glfw.SetCharCallback(window, (_, codepoint) => glfwInput.CharCallback(codepoint));

		(int windowX, int windowY) = glfw.GetInitialWindowPos(WindowConstants.WindowWidth, WindowConstants.WindowHeight);
		glfw.SetWindowPos(window, windowX, windowY);

		glfw.MakeContextCurrent(window);
		glfw.SetWindowSizeLimits(window, 1024, 768, -1, -1);

		glfw.SwapInterval(1);

		return window;
	}

	[Factory(Scope.SingleInstance)]
	private static GL GetGl(Glfw glfw)
	{
		return GL.GetApi(glfw.GetProcAddress);
	}

	[Factory(Scope.SingleInstance)]
	private static ImGuiController CreateImGuiController(GL gl, GlfwInput glfwInput)
	{
		ImGuiController imGuiController = new(gl, glfwInput, WindowConstants.WindowWidth, WindowConstants.WindowHeight);
		imGuiController.CreateDefaultFont();
		return imGuiController;
	}

	[Factory(Scope.SingleInstance)]
	private static IReadOnlyList<WindowBase> GetWindows(
		FileSystemRenameWindow fileSystemRenameWindow,
		HtmlChangeDetectionWindow htmlChangeDetectionWindow,
		NetworkWindow networkWindow,
		RandomGeneratorWindow randomGeneratorWindow)
	{
		return [fileSystemRenameWindow, htmlChangeDetectionWindow, networkWindow, randomGeneratorWindow];
	}
}
