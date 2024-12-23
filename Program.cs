using EncryptionTool.cmd;
using EncryptionTool.utils;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

var command = ArgumentsParser.GetCommand(args, out var config);

var cli = new Cli(command, config);
cli.Run();
