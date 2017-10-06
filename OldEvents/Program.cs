using System;

namespace OldEvents
{
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Timers;

    class Program
    {
        public delegate void MyAction(string message);
        public static event MyAction OnMyAction;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello RX");

            //Sample1();
            //Sample2();
            //Sample3();
            //Sample4();

            Console.ReadLine();
        }   

        private static void Sample4()
        {
            var source1 = Observable.Range(1, 10);
            var source2 = Observable.Range(1, 20);
            var source3 = Observable.Range(20, 5);

            var allTogether = source1.Concat(source2).Concat(source3);

            var distinct = allTogether.Distinct();
            distinct.Subscribe(number => Console.WriteLine($"number: {number}"));

        }

        private static void Sample3()
        {
            var interval = Observable.Range(1,10);

            var sum = interval.Aggregate(0, (acc, currentValue) => acc + currentValue);
            var oddNumber = interval.Scan(0, (acc, current) => current % 2 != 0 ? current : -1)
                .Where(q => q > 0)
                .Skip(1)
                .SkipLast(1);

            interval.Subscribe(number => Console.WriteLine($"liczba: {number}"));
            sum.Subscribe(s => Console.WriteLine($"sum: {s}"));
            oddNumber.Subscribe(o => Console.WriteLine($"nieparzysta liczba: {o}"));
            //sum.Dump("suma");

        }
        private static void Sample2()
        {
            //sequences
            var range = Observable.Range(1, 10);
            range.Subscribe(a => Console.WriteLine($"range sequence: {a}"), () => Console.WriteLine("end"));

            var interval = Observable.Interval(TimeSpan.FromSeconds(1));
            interval.Subscribe(v => Console.WriteLine(v % 2 == 0 ? "tik" : "tak"), () => Console.WriteLine("completed"));

            var naturalNumbers = Observable.Create<long>(
                observer =>
                    {
                        var timer = new System.Timers.Timer
                        {
                            Interval = 1000
                        };
                        timer.Elapsed += (s, e) => observer.OnNext(NaturalNumber++);
                        timer.Start();
                        return () =>
                            {
                                timer.Dispose();
                            };
                    });
            naturalNumbers.Subscribe(nn => Console.WriteLine($"natural number: {nn}"));
        }

        private static long NaturalNumber = 0;      

        private static void Sample1()
        {
            //how to dispose anonymous, to avoid memory leaks
            OnMyAction += (msg) => { Console.WriteLine(msg); };

            MyAction action = (msg) => { Console.WriteLine($"action: {msg}"); };

            OnMyAction += action; //attach
            //....
            OnMyAction -= action; //disposing

            OnMyAction?.Invoke("what's wrong");

            //the new way, low level
            var subject = new Subject<string>(); /*
                                                        public delegate void MyAction(string message);
                                                        public static event MyAction OnMyAction;
                                                    */
            subject.Subscribe(Console.WriteLine); //  OnMyAction += action;//attach
            subject.OnNext("hello from subject"); //  OnMyAction?.Invoke("...");
            subject.OnCompleted(); //  OnMyAction -= action;//disposing
            subject.Dispose(); //  dispose

            //new, better, simple
            Observable.Create<string>(
                observer =>
                    {
                        observer.OnNext("hello from observable");
                        //observer.OnError(new TimeoutException("nie mam czasu"));
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }).Subscribe(
                (msg) => { Console.WriteLine(msg); },
                ex => Console.WriteLine($"error: {ex.Message}"),
                () => { Console.WriteLine("observable completed"); });
        }
    }

    public static class SampleExtentions
    {
        public static void Dump<T>(this IObservable<T> source, string name)
        {
            source.Subscribe(
                i => Console.WriteLine("{0}-->{1}", name, i),
                ex => Console.WriteLine("{0} errored-->{1}", name, ex.Message),
                () => Console.WriteLine("{0} completed", name));
        }
    }
}
