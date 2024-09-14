using Detach;
using Detach.Numerics;
using ImGuiNET;
using Silk.NET.GLFW;
using System.Net;
using System.Net.NetworkInformation;

namespace MiscTools.Services.Ui;

public sealed class NetworkWindow : WindowBase
{
	private readonly Glfw _glfw;

	private readonly IPGlobalProperties _ipProperties = IPGlobalProperties.GetIPGlobalProperties();
	private readonly Dictionary<IPAddress, Rgb> _ipAddressColorLookup = new();
	private readonly Dictionary<int, Rgb> _ipAddressPortColorLookup = new();
	private readonly Dictionary<TcpState, Rgb> _tcpColorLookup = new();
	private readonly IReadOnlyList<Rgb> _colors =
	[
		new(0, 0, 255),
		new(0, 255, 0),
		new(255, 0, 0),
		new(255, 255, 0),
		new(255, 0, 255),
		new(0, 255, 255),
		new(127, 255, 0),
		new(255, 127, 0),
		new(0, 127, 255),
		new(127, 0, 255),
		new(255, 0, 127),
		new(0, 255, 127),
		new(127, 127, 255),
		new(127, 255, 127),
		new(255, 127, 127),
		new(127, 127, 127),
		new(255, 255, 255),
	];

	private TcpConnectionInformation[] _connections = [];
	private double _timer;
	private bool _showLocalhost;

	private uint _sorting;
	private bool _sortAscending;

	public NetworkWindow(Glfw glfw)
	{
		_glfw = glfw;
	}

	public override unsafe void Render()
	{
		_timer += _glfw.GetTime();
		if (_timer > 63)
		{
			FetchConnections();
			_timer = 0;
		}

		if (ImGui.Begin("Network"))
		{
			if (ImGui.Checkbox("Show localhost", ref _showLocalhost))
				_timer = 0;

			if (ImGui.BeginTable("NetworkTable", 5, ImGuiTableFlags.Sortable))
			{
				ImGui.TableSetupColumn("Local endpoint address", ImGuiTableColumnFlags.None, 0, 0);
				ImGui.TableSetupColumn("Local endpoint port", ImGuiTableColumnFlags.None, 0, 1);
				ImGui.TableSetupColumn("Remote endpoint address", ImGuiTableColumnFlags.None, 0, 2);
				ImGui.TableSetupColumn("Remote endpoint port", ImGuiTableColumnFlags.None, 0, 3);
				ImGui.TableSetupColumn("State", ImGuiTableColumnFlags.None, 0, 4);
				ImGui.TableHeadersRow();

				ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
				if (sortsSpecs.NativePtr != null && sortsSpecs.SpecsDirty)
				{
					_sorting = sortsSpecs.Specs.ColumnUserID;
					_sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;
					SortConnections();

					sortsSpecs.SpecsDirty = false;
				}

				ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
				clipper.Begin(_connections.Length);
				while (clipper.Step())
				{
					for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
					{
						ImGui.TableNextRow();

						TcpConnectionInformation connection = _connections[i];
						NextColumn(connection.LocalEndPoint.Address, _ipAddressColorLookup);
						NextColumn(connection.LocalEndPoint.Port, _ipAddressPortColorLookup);
						NextColumn(connection.RemoteEndPoint.Address, _ipAddressColorLookup);
						NextColumn(connection.RemoteEndPoint.Port, _ipAddressPortColorLookup);
						NextColumn(connection.State, _tcpColorLookup);
					}
				}

				clipper.End();

				ImGui.EndTable();
			}
		}

		ImGui.End();
	}

	private void NextColumn<T>(T key, Dictionary<T, Rgb> lookup)
		where T : notnull
	{
		string text = key.ToString() ?? string.Empty;

		ImGui.TableNextColumn();
		ImGui.TextColored(GetColor(key, lookup), text);
		ImGui.SameLine();
		if (ImGui.SmallButton(Inline.Span($"Copy##{text}")))
			ImGui.SetClipboardText(key.ToString());
	}

	private Rgb GetColor<T>(T key, Dictionary<T, Rgb> lookup)
		where T : notnull
	{
		if (lookup.TryGetValue(key, out Rgb color))
			return color;

		color = _colors[lookup.Count % _colors.Count];
		lookup[key] = color;
		return color;
	}

	private void FetchConnections()
	{
		_connections = _ipProperties.GetActiveTcpConnections();
		if (!_showLocalhost)
			_connections = _connections.Where(tci => !(IsLocalHost(tci.LocalEndPoint) && IsLocalHost(tci.RemoteEndPoint))).ToArray();

		SortConnections();
	}

	private void SortConnections()
	{
		_connections = _sorting switch
		{
			0 => _sortAscending ? _connections.OrderBy(tci => tci.LocalEndPoint.Address.ToString()).ToArray() : _connections.OrderByDescending(tci => tci.LocalEndPoint.Address.ToString()).ToArray(),
			1 => _sortAscending ? _connections.OrderBy(tci => tci.LocalEndPoint.Port).ToArray() : _connections.OrderByDescending(tci => tci.LocalEndPoint.Port).ToArray(),
			2 => _sortAscending ? _connections.OrderBy(tci => tci.RemoteEndPoint.Address.ToString()).ToArray() : _connections.OrderByDescending(tci => tci.RemoteEndPoint.Address.ToString()).ToArray(),
			3 => _sortAscending ? _connections.OrderBy(tci => tci.RemoteEndPoint.Port).ToArray() : _connections.OrderByDescending(tci => tci.RemoteEndPoint.Port).ToArray(),
			4 => _sortAscending ? _connections.OrderBy(tci => tci.State).ToArray() : _connections.OrderByDescending(tci => tci.State).ToArray(),
			_ => _connections,
		};
	}

	private static bool IsLocalHost(IPEndPoint ipEndPoint)
	{
		const string localHostIpV4 = "127.0.0.1";
		const string localHostIpV6 = "::1";

		string addressString = ipEndPoint.Address.ToString();
		return addressString.StartsWith(localHostIpV4) || addressString.StartsWith(localHostIpV6);
	}
}
