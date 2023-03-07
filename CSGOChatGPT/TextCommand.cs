using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOChatGPT;
public class TextCommand {
    public string Command;
    public string Description;
    public Action<string> Action;
    public bool UseCommandAsStringSplitter;

    public TextCommand(string command, string description, Action<string> action) {
        Command = command.ToLower();
        Description = description;
        Action = action;
    }

    public bool IsCommand(string text) {
        if (UseCommandAsStringSplitter) {
            return text.ToLower().Contains(Command);
        }
        return text.ToLower().StartsWith(Command);
    }

    public void Execute(string text) {
        int index = text.ToLower().IndexOf(Command);
        string parameter = text.Substring(index + Command.Length).Trim();

        try {
            Action(parameter);
        } catch (Exception e) {
            Console.WriteLine($"Error executing command {Command}: " + e.Message);
        }
    }
}

public class CommandSet {
    public string Description { get; set; }
    private List<TextCommand> commands = new List<TextCommand>();

    public CommandSet(string description = "Commands:") { 
        Description = description;
    }

    public void AddCommand(TextCommand command) {
        commands.Add(command);
    }

    public void AddCommand(string command, string description, Action<string> action) {
        commands.Add(new TextCommand(command, description, action));
    }
    public void AddSplitterCommand(string command, string description, Action<string> action) {
        commands.Add(new TextCommand(command, description, action) { UseCommandAsStringSplitter = true });
    }

    public bool ExecuteCommand(string text) {
        if (string.IsNullOrWhiteSpace(text)) {
            return false;
        }

        foreach (TextCommand command in commands) {
            if (command.IsCommand(text)) {
                command.Execute(text);
                return true;
            }
        }

        return false;
    }

    public override string ToString() {
        return $"{Description}\n" + string.Join("\n", commands.Select(c => $"    {c.Command} - {c.Description}"));
    }
}
