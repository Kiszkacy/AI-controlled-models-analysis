using System;
using System.IO.Pipes;

using Godot;

public class PipeHandler : Singleton<PipeHandler>
{
    public string PipeName
    {
        get => this.pipeName;
        set
        {
            if (this.IsConnected) throw new Exception("Already connected! Disconnect first to change pipe name.");
            this.pipeName = value;
        }
    }

    private string pipeName = Config.Get().Data.Pipe.Name;
    private NamedPipeClientStream pipe;
    private readonly int ReadBufferSize = Config.Get().Data.Pipe.BufferSize;
    private bool IsConnected { get; set; } = false;

    public void Connect()
    {
        this.pipe = new NamedPipeClientStream(".", this.pipeName, PipeDirection.InOut);
        this.pipe.Connect();
        this.IsConnected = true;
        NeatPrinter.Start().Print($"[PIPE]  | Connected to '{this.pipeName}' pipe.").End();
    }

    public void Disconnect()
    {
        this.pipe.Close();
        this.IsConnected = false;
        NeatPrinter.Start().Print($"[PIPE]  | Disconnected from '{this.pipeName}' pipe.").End();
    }

    public void Send(byte[] data) => this.pipe.Write(data, 0, data.Length);

    public byte[] Receive()
    {
        byte[] buffer = new byte[ReadBufferSize];
        int readBytes = this.pipe.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    private PipeHandler()
    {

    }
}