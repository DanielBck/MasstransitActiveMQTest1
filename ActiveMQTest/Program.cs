using MassTransit;
using MassTransit.ActiveMqTransport;
using System;

namespace ActiveMQTest
{
    public class Program
    {
        public interface IMyMessage
        {            
            string Text { get; }
        }        

        public class MyMessage : IMyMessage
        {
            public string Text { get; set; }
        }        

        static void Main(string[] args)
        {
            TestUsingActiveMQTransport();
            //TestUsingInMemoryTransport();
        }

        private static void TestUsingActiveMQTransport()
        {
            var bus = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                var host = cfg.Host("XXXXXXXXXXXXXXXXXXX.amazonaws.com", 61617, h =>
                {
                    h.Username("ActiveMQUserName");
                    h.Password("ActiveMQPassword");

                    h.UseSsl();

                });

                cfg.ReceiveEndpoint(host, "test_queue",
                    ep => { SetupHandlers(ep); });
            });

            bus.Start();

            PublishMessage(bus);

            Console.ReadLine();

            bus.Stop();
        }

        private static void TestUsingInMemoryTransport()
        {
            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("test_queue",
                    ep =>
                    {
                        SetupHandlers(ep);
                    });
            });

            bus.Start();

            PublishMessage(bus);

            Console.ReadLine();

            bus.Stop();
        }

        private static void PublishMessage(IBusControl bus)
        {
            Console.WriteLine("Publishing message");                    
            bus.Publish(new MyMessage {Text = "Hello"});            
        }

        private static void SetupHandlers(IReceiveEndpointConfigurator ep)
        {
            ep.Handler<IMyMessage>(context => { return Console.Out.WriteLineAsync($"Received MyMessage: {context.Message.Text}"); });
        }        
    }
}
