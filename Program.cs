using EncryptionTool.cmd;
using EncryptionTool.services;
using EncryptionTool.utils;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

var config = new Lazy<ConfigService>(() => new ConfigService());

var command = ArgumentsParser.GetCommand(args, config);
var cli = new Cli(command, config.Value);
cli.Run();
