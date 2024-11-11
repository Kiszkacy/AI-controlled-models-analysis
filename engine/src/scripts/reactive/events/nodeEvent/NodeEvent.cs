using Godot;

public record NodeEvent(Node Node) : Event<Node>(Node);