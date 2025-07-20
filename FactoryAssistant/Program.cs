// Import packages
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

var configBuilder = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json")
	.Build();

// Populate values from your OpenAI deployment
var modelId = configBuilder.GetSection("Settings:model-id").Value;
var endpoint = configBuilder.GetSection("Settings:ai-endpoint").Value;
var apiKey = configBuilder.GetSection("Settings:ai-key").Value;


// Create a kernel with Azure OpenAI chat completion
var kernelBuilder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

// Add enterprise components
kernelBuilder.Services.AddLogging(services => services.SetMinimumLevel(LogLevel.Trace));

// Build the kernel
Kernel kernel = kernelBuilder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add a plugin (the LightsPlugin class is defined below)
kernel.Plugins.AddFromType<LightsPlugin>("Lights");
kernel.Plugins.AddFromType<MachineFlagsPlugin>("Machines");

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
	FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create a history store the conversation
var history = new ChatHistory();

// Initiate a back-and-forth chat
string? userInput;
do
{
	// Collect user input
	Console.Write("User > ");
	userInput = Console.ReadLine();

	// Add user input
	history.AddUserMessage(userInput);

	// Get the response from the AI
	var result = await chatCompletionService.GetChatMessageContentAsync(
		history,
		executionSettings: openAIPromptExecutionSettings,
		kernel: kernel);

	// Print the results
	Console.WriteLine("Assistant > " + result);

	// Add the message from the agent to the chat history
	history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);