using client;

try
{
    Client newClient = new Client();
    await newClient.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
