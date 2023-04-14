using client;

try
{
    ClientSession newClient = new ClientSession();
    await newClient.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
