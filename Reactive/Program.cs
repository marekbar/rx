using System;

namespace Reactive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.IO.Packaging;
    using System.Net.Mime;
    using System.Net.Sockets;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reactive.Threading.Tasks;
    using System.Runtime.ExceptionServices;
    using System.Runtime.Remoting.Metadata.W3cXsd2001;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;

    /*
     * Reactive
     * http://www.pzielinski.com/?cat=27
     */
    class Program
    {


        static void Main(string[] args)
        {
            //RemoveAnonymousEventSample();

            //ImplementingObserverAndObservable();

            //SubjectSample1();

            //ReplaySubject();
            //ReplaySubjectBufferExample();
            //ReplaySubjectWindowExample();
            //BehaviorSubjectExample();
            //BehaviorSubjectExample2();
            //BehaviorSubjectExample3();
            //BehaviorSubjectCompletedExample();
            //AsyncSubjectSample();
            //AsyncSubjectWithCompleteSample();
            //SubjectInvalidUsageExample();
            //SubjectSubscribeWithError();
            //SubjectUnsubscribing();
            //NonBlocking_event_driven();

            //ObservableRange();
            //ObservableRangeBuiltOnGenerate();
            //OservableInterval();
            //ObservableTime();
            //ObservableWithInfiniteNaturalNumbers();
            //StartAction();
            //StartFunc();

            //ToObservable();

            // SimpleWhere();
            //Distinct();
            //DistinctUntilChanged();
            //IgnoreElements();
            //SkipAndTake();
            //TakeWithInterval();
            //SkipWhile();
            //TakeWhile();
            //SkipLast();
            //SkipUntil();
            //TakeUntil();
            //Any();
            //All();
            //Empty();
            //Contains();
            //DefaultIfEmpty();
            //ElementAt();
            //SequenceEqual();

            //Agregations
            //Count();
            //MinMaxSumAverage();
            //MinMaxSumAverageOnObjects();
            //AggregateCustomAverage();
            //MinimumWithScanComparer();
            //GroupBy();
            //Select();
            //Select2();
            //Cast();
            //OfType();
            //TimeIntervalSample();
            //TimeStampSample();
            //Materialize();
            //SelectMany();
            //SelectManyWithWhere();
            //CatchTimeoutException();
            //ConcatSequences();
            //RepeatSequence();
            //Zip();
            //ThreadSample();
            //SubscribeOn();

            // PassingState();
            //FutureScheduling();

            //Cancellation();
            //CancellRecursive();
            //BufferSample();
            //BufferSample2();
            //OverlappingBuffersByTime();
            //Sample_reduce_data();

            //Throttle();
            //SimpleColdSample();
            //SimpleConnectSample();
            //ConnectAndDisposeSample();
            RefCountExample();
            Console.ReadLine();
        }

        private static void RefCountExample()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
                .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
                .Publish()
                .RefCount();
            //observable.Connect(); Use RefCount instead now 
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            /* Output: 
            Press any key to subscribe 
            Press any key to unsubscribe. 
            Publishing 0 
            subscription : 0 
            Publishing 1 
            subscription : 1 
            Publishing 2 
            subscription : 2 
            Press any key to exit. 
            */
        }

        private static void ConnectAndDisposeSample()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Publish();
            observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Press enter to connect, esc to exit.");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    var connection = observable.Connect(); //--Connects here--
                    Console.WriteLine("Press any key to dispose of connection.");
                    Console.ReadKey();
                    connection.Dispose(); //--Disconnects here--
                }
                if (key.Key == ConsoleKey.Escape)
                {
                    exit = true;
                }
            }
            /* Output: 
            Press enter to connect, esc to exit. 
            Press any key to dispose of connection. 
            subscription : 0 
            subscription : 1 
            subscription : 2 
            Press enter to connect, esc to exit. 
            Press any key to dispose of connection. 
            subscription : 0 
            subscription : 1 
            subscription : 2 
            Press enter to connect, esc to exit. 
            */
        }
        private static void SimpleConnectSample()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Publish();

            observable.Connect();

            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            
            Console.ReadKey();
            /* Output: 
            first subscription : 0 
            first subscription : 1 
            second subscription : 1 
            first subscription : 2 
            second subscription : 2 
            */
        }

        private static void SimpleColdSample()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period);
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            Console.ReadKey();
            /* Output: 
            first subscription : 0 
            first subscription : 1 
            second subscription : 0 
            first subscription : 2 
            second subscription : 1 
            first subscription : 3 
            second subscription : 2 
            */
        }

        private static void Throttle()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(1500));
            interval.Throttle(TimeSpan.FromSeconds(1)).Subscribe(Console.WriteLine);
        }

        private static void Sample_reduce_data()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
            interval.Sample(TimeSpan.FromSeconds(1)).Subscribe(Console.WriteLine);
        }

        private static void OverlappingBuffersByTime()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10);
            var overlapped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1));
            var standard = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
            var skipped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5));
            overlapped.Subscribe(
                buffer =>
                    {
                        Console.WriteLine("--Overlapped values");
                        foreach (var value in buffer)
                        {
                            Console.WriteLine(value);
                        }
                    }, () => Console.WriteLine("Completed"));
        }

        private static void BufferSample2()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10);
            source.Buffer(3, 1)//bufer last three, skip 1
                .Subscribe(
                    buffer =>
                        {
                            Console.WriteLine("--Buffered values");
                            foreach (var value in buffer)
                            {
                                Console.WriteLine(value);
                            }
                        }, () => Console.WriteLine("Completed"));
        }

        private static void BufferSample()
        {
            var idealBatchSize = 15;
            var maxTimeDelay = TimeSpan.FromSeconds(3);

            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10)
                .Concat(Observable.Interval(TimeSpan.FromSeconds(0.01)).Take(100));

            source.Buffer(maxTimeDelay, idealBatchSize)
                .Subscribe(
                    buffer => Console.WriteLine("Buffer of {1} @ {0}", DateTime.Now, buffer.Count),
                    () => Console.WriteLine("Completed"));
        }

        private static void CancellRecursive()
        {
            Action<Action> work = (Action self)//will be tranformed to loop internally
                =>
                {
                    Console.WriteLine("Running");
                    self();
                };

            var token = new NewThreadScheduler().Schedule(work);
            Console.ReadLine();
            Console.WriteLine("Cancelling");
            token.Dispose();
            Console.WriteLine("Cancelled");
        }

        private static void Cancellation()
        {
            var scheduler = new NewThreadScheduler();
            var delay = TimeSpan.FromSeconds(1);
            Console.WriteLine("Before schedule at {0:o}", DateTime.Now);
            var token = scheduler.Schedule(delay,
                () => Console.WriteLine("Inside schedule at {0:o}", DateTime.Now));
            Console.WriteLine("After schedule at  {0:o}", DateTime.Now);
            token.Dispose();//remove from queue of work

            //cancel sth is running
            var list = new List<int>();
            Console.WriteLine("Enter to quit");
            var token2 = scheduler.Schedule(list, Work);
            Console.WriteLine("Press enter to cancel");
            Console.ReadLine();
            Console.WriteLine("cancelling...");
            token2.Dispose();
            Console.WriteLine("Cancelled");
        }

        private static IDisposable Work(IScheduler scheduler, List<int> list)
        {
            var tokenSource = new CancellationTokenSource();
            var cancelToken = tokenSource.Token;
            var task = new Task(() =>
                {
                    Console.WriteLine();
                    for (int i = 0; i < 1000; i++)
                    {
                        var sw = new SpinWait();
                        for (int j = 0; j < 3000; j++) sw.SpinOnce();
                        Console.Write(".");
                        list.Add(i);
                        if (cancelToken.IsCancellationRequested)
                        {
                            Console.WriteLine("Cancelation requested");
                            //cancelToken.ThrowIfCancellationRequested();
                            return;
                        }
                    }
                }, cancelToken);
            task.Start();
            return Disposable.Create(tokenSource.Cancel);
        }

        private static void FutureScheduling()
        {
            var scheduler = new NewThreadScheduler();
            var delay = TimeSpan.FromSeconds(1);
            Console.WriteLine("Before schedule at {0:o}", DateTime.Now);
            scheduler.Schedule(delay,
                () => Console.WriteLine("Inside schedule at {0:o}", DateTime.Now));
            Console.WriteLine("After schedule at  {0:o}", DateTime.Now);
        }

        private static void PassingState()
        {             
            var scheduler = new NewThreadScheduler();
            var myName = "Lee";

            //WRONG WAY
            //scheduler.Schedule(() => { Console.WriteLine($"{nameof(myName)} = {myName}"); });

            //CORRECT
            scheduler.Schedule(myName,
                (_, state) =>
                    {
                        Console.WriteLine(state);
                        return Disposable.Empty;
                    });
            myName = "John";

            //DO NOT DO THAT, shared list was modified            
            var list = new List<int>();
            scheduler.Schedule(list,
                (innerScheduler, state) =>
                    {
                        Console.WriteLine(state.Count);
                        return Disposable.Empty;
                    });
            list.Add(1);
        }

        private static void SubscribeOn()
        {
            Console.WriteLine("Starting on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
            var source = Observable.Create<int>(
                o =>
                    {
                        Console.WriteLine("Invoked on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                        o.OnNext(1);
                        o.OnNext(2);
                        o.OnNext(3);
                        o.OnCompleted();
                        Console.WriteLine("Finished on threadId:{0}",
                            Thread.CurrentThread.ManagedThreadId);
                        return Disposable.Empty;
                    });
            source
                
                .SubscribeOn(Scheduler.Default)
                //.ObserveOn(DispatcherScheduler.Current) //in console app there's no dispatcher
                .Subscribe(
                    o => Console.WriteLine("Received {1} on threadId:{0}",
                        Thread.CurrentThread.ManagedThreadId,
                        o),
                    () => Console.WriteLine("OnCompleted on threadId:{0}",
                        Thread.CurrentThread.ManagedThreadId));

            Console.WriteLine("Subscribed on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
        }

        private static void ThreadSample()
        {
            Console.WriteLine($"Starting on thread ID: {Thread.CurrentThread.ManagedThreadId}");

            var subject = new Subject<object>();

            subject.Subscribe(
                o => Console.WriteLine($"Received {Thread.CurrentThread.ManagedThreadId} on thread ID: {o}"));

            ParameterizedThreadStart notify = obj =>
                {
                    Console.WriteLine($"OnNext({Thread.CurrentThread.ManagedThreadId}) on thread ID: {obj}");
                    subject.OnNext(obj);
                };
            notify(1);

            new Thread(notify).Start(2);
            new Thread(notify).Start(3);
        }

        private static void Zip()
        {
            //Generate values 0,1,2 
            var nums = Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Take(3);
            //Generate values a,b,c,d,e,f 
            var chars = Observable.Interval(TimeSpan.FromMilliseconds(150))
                .Take(6)
                .Select(i => Char.ConvertFromUtf32((int)i + 97));
            nums
                .Zip(chars, (lhs, rhs) => new { Left = lhs, Right = rhs })
                .Subscribe(Console.WriteLine);
        }

        private static void RepeatSequence()
        {
            Observable.Range(1, 3).Repeat(3).Subscribe(Console.WriteLine);
        }

        private static void ConcatSequences()
        {
            var sequenceOne = Observable.Interval(TimeSpan.FromSeconds(1));
            var sequenceTwo = Observable.Interval(TimeSpan.FromSeconds(1));

            var sequence = sequenceOne.Concat(sequenceTwo);
            sequence.Subscribe(Console.WriteLine);
        }

        private static void CatchTimeoutException()
        {
            var source = new Subject<int>();
            var result = source
                .Retry(3)//retry after 1 error occured - beware that crap
                .Catch<int, TimeoutException>(tx => Observable.Return(-1))
                .Catch<int, Exception>(ex => Observable.Return(-999))
            .Finally(() => Console.WriteLine("Finally"));

            result.Subscribe(
                Console.WriteLine,
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnError(new Exception("ohh no"));//OnError ends subscription
            source.OnNext(2);
            source.OnError(new TimeoutException());
            source.OnNext(33);

        }

        private static void SelectManyWithWhere()
        {
            Func<int, char> letter = i => (char)(i + 64);
            Observable.Range(1, 30).Where(i => 0 < i && i < 27).SelectMany(i => Observable.Return(letter(i))).Subscribe(Console.WriteLine);
        }

        private static void SelectMany()
        {
            Func<int, char> letter = i => (char)(i + 64);
            Observable.Range(1, 30)
                .SelectMany(
                    i =>
                        {
                            if (0 < i && i < 27)
                            {
                                return Observable.Return(letter(i));
                            }
                            else
                            {
                                return Observable.Empty<char>();
                            }
                        })
                .Dump("SelectMany");
        }

        private static void Materialize()
        {
            Observable.Range(1, 3)
                .Materialize()
                .Dump("Materialize");
        }

        private static void TimeStampSample()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(3)
                .Timestamp()
                .Dump("TimeStamp");
        }

        private static void TimeIntervalSample()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(3)
                .TimeInterval()
                .Dump("TimeInterval");
        }

        private static void OfType()
        {
            var objects = new Subject<object>();
            objects.OfType<int>().Dump("OfType");//source.Where(i=>i is int).Select(i=>(int)i);
            objects.OnNext(1);
            objects.OnNext(2);
            objects.OnNext("3");//Ignored
            objects.OnNext(4);
            objects.OnCompleted();
        }

        private static void Cast()
        {
            //source.Cast<int>(); is equivalent to
            //source.Select(i => (int)i);
            var objects = new Subject<object>();
            objects.Cast<int>().Dump("cast");
            objects.OnNext(1);
            objects.OnNext(2);
            objects.OnNext("3");//Fail
        }

        private static void Select2()
        {
            Observable.Range(1, 5)
                .Select(
                    i => new { Number = i, Character = (char)(i + 64) })
                .Dump("anon");
        }

        private static void Select()
        {
            var source = Observable.Range(0, 5);
            source.Select(i => i + 3).Dump("+3");
        }

        private static void GroupBy()
        {
            var persons = new Subject<Person>();
            persons.Dump("persons");
            var group = persons.GroupBy(g => g.Age);

            //bullshit groupping
            group.Subscribe(
                grp => { grp.Min().Subscribe(minValue => Console.WriteLine("{0} min value = {1}", grp.Key, minValue)); }, () => Console.WriteLine("groupping completed"));

            persons.OnNext(new Person("John", 23));
            persons.OnNext(new Person("Paul", 32));
            persons.OnNext(new Person("Joanna", 21));
            persons.OnNext(new Person("Carl", 19));



            persons.OnCompleted();
        }

        private static void MinimumWithScanComparer()
        {
            var persons = new Subject<Person>();
            var comparer = Comparer<Person>.Default;
            Func<Person, Person, Person> minOf = (x, y) => comparer.Compare(x, y) < 0 ? x : y;
            var min = persons.Scan<Person>(minOf).DistinctUntilChanged();
            min.Subscribe(Console.WriteLine, () => Console.WriteLine("min scan completed"));

            persons.Dump("persons");

            persons.OnNext(new Person("John", 23));
            persons.OnNext(new Person("Paul", 32));
            persons.OnNext(new Person("Joanna", 21));
            persons.OnNext(new Person("Carl", 19));



            persons.OnCompleted();
        }

        private static void AggregateCustomAverage()
        {
            var persons = new Subject<Person>();
            persons.Dump("persons");

            var sum = persons.Aggregate(0, (acc, currentValue) => acc + currentValue.Age);
            var count = persons.Aggregate(0, (acc, currentValue) => acc + 1);//++ does not work
            sum.Dump("sum");
            count.Dump("count");


            persons.OnNext(new Person("John", 23));
            persons.OnNext(new Person("Paul", 32));
            persons.OnNext(new Person("Joanna", 21));

            persons.OnCompleted();
        }

        private static void MinMaxSumAverageOnObjects()
        {
            var persons = new Subject<Person>();
            persons.Dump("persons");
            persons.Min().Dump("min");
            persons.Max().Dump("max");
            //persons.Average().Dump("average");sucks
            persons.OnNext(new Person("John", 23));
            persons.OnNext(new Person("Paul", 32));
            persons.OnNext(new Person("Joanna", 21));


            persons.OnCompleted();
        }

        private static void MinMaxSumAverage()
        {
            var numbers = new Subject<int>();
            numbers.Dump("numbers");
            numbers.Min().Dump("Min");
            numbers.Average().Dump("Average");
            numbers.Sum().Dump("Sum");

            numbers.OnNext(1);
            numbers.OnNext(2);
            numbers.OnNext(3);
            numbers.OnCompleted();
        }

        private static void Count()
        {
            var numbers = Observable.Range(0, 3);
            numbers.Dump("numbers");
            numbers.Count().Dump("count");
        }

        private static void SequenceEqual()
        {
            var subject1 = new Subject<int>();

            subject1.Subscribe(
                i => Console.WriteLine("subject1.OnNext({0})", i),
                () => Console.WriteLine("subject1 completed"));

            var subject2 = new Subject<int>();

            subject2.Subscribe(
                i => Console.WriteLine("subject2.OnNext({0})", i),
                () => Console.WriteLine("subject2 completed"));

            var areEqual = subject1.SequenceEqual(subject2);

            areEqual.Subscribe(
                i => Console.WriteLine("areEqual.OnNext({0})", i),
                () => Console.WriteLine("areEqual completed"));

            subject1.OnNext(1);
            subject1.OnNext(2);

            subject2.OnNext(1);
            subject2.OnNext(2);

            subject2.OnNext(3);
            subject1.OnNext(3);

            subject1.OnCompleted();
            subject2.OnCompleted();
        }

        private static void ElementAt()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var elementAt1 = subject.ElementAt(1);//use ElementAtOrDefault to avoid errors
            elementAt1.Subscribe(
                b => Console.WriteLine("elementAt1 value: {0}", b),
                () => Console.WriteLine("elementAt1 completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        private static void DefaultIfEmpty()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe(
                b => Console.WriteLine("defaultIfEmpty value: {0}", b),
                () => Console.WriteLine("defaultIfEmpty completed"));
            var default42IfEmpty = subject.DefaultIfEmpty(42);
            default42IfEmpty.Subscribe(
                b => Console.WriteLine("default42IfEmpty value: {0}", b),
                () => Console.WriteLine("default42IfEmpty completed"));
            subject.OnCompleted();
        }

        private static void Contains()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var contains = subject.Contains(2);
            contains.Subscribe(
                b => Console.WriteLine("Contains the value 2? {0}", b),
                () => Console.WriteLine("contains completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        private static void Empty()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var isEmpty = subject.All(_ => false);
            isEmpty.Subscribe(v => Console.WriteLine("brak danych"));

            subject.OnCompleted();
        }
        private static void All()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);//will show when first value is less than 5
            all.Subscribe(b => Console.WriteLine("All values less than 5? {0}", b));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(6);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();
        }

        private static void Any()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var any = subject.Any();
            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b));
            subject.OnNext(1);
            subject.OnCompleted();
        }

        private static void TakeUntil()
        {
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
                .TakeUntil(otherSubject)//until secontary sequence start to produce
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);
            subject.OnCompleted();
        }

        private static void SkipUntil()
        {
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
                .SkipUntil(otherSubject)//until second observable will produce same value
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);
            subject.OnCompleted();
        }

        private static void SkipLast()
        {
            var subject = new Subject<int>();
            subject.SkipLast(2).Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Console.WriteLine("Pushing 1");
            subject.OnNext(1);
            Console.WriteLine("Pushing 2");
            subject.OnNext(2);
            Console.WriteLine("Pushing 3");
            subject.OnNext(3);
            Console.WriteLine("Pushing 4");
            subject.OnNext(4);
            subject.OnCompleted();
        }

        private static void TakeWhile()
        {
            var subject = new Subject<int>();
            subject
                .TakeWhile(i => i < 4)//takes 3 first values
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(0);
            subject.OnCompleted();
        }

        private static void SkipWhile()
        {
            var subject = new Subject<int>();
            subject
                .SkipWhile(i => i < 4)//skips 3 first values
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(0);
            subject.OnCompleted();
        }

        private static void TakeWithInterval()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(1000)).Take(3).Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        private static void SkipAndTake()
        {
            Observable.Range(0, 10).Skip(3).Take(4).Subscribe(Console.WriteLine, () => Console.WriteLine("completed"));
        }

        private static void IgnoreElements()
        {
            var subject = new Subject<int>();
            //Could use subject.Where(_=>false);
            var noElements = subject.IgnoreElements();
            subject.Subscribe(
                i => Console.WriteLine("subject.OnNext({0})", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            noElements.Subscribe(
                i => Console.WriteLine("noElements.OnNext({0})", i),
                () => Console.WriteLine("noElements.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        private static void DistinctUntilChanged()
        {
            var subject = new Subject<int>();
            var distinct = subject.DistinctUntilChanged();
            subject.Subscribe(
                i => Console.WriteLine("{0}", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            distinct.Subscribe(
                i => Console.WriteLine("distinct.OnNext({0})", i),
                () => Console.WriteLine("distinct.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        private static void Distinct()
        {
            var subject = new Subject<int>();
            var distinct = subject.Distinct();
            subject.Subscribe(
                i => Console.WriteLine("{0}", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            distinct.Subscribe(
                i => Console.WriteLine("distinct.OnNext({0})", i),
                () => Console.WriteLine("distinct.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        private static void SimpleWhere()
        {
            var oddNumbers = Observable.Range(0, 10)
                .Where(i => i % 2 == 0)
                .Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }

        private static void ToObservable()
        {
            var t = Task.Factory.StartNew(() => "Test");
            var t2 = Task.Factory.StartNew(() => "Test 2");
            var source = t.ToObservable();
            var source2 = t2.ToObservable();
            var join = source.Concat(source2);

            join.Subscribe(Console.WriteLine, () => Console.WriteLine("completed"));
        }

        private static void StartAction()
        {
            var start = Observable.Start(() =>
                {
                    Console.Write("Working away");
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(100);
                        Console.Write(".");
                    }
                });
            start.Subscribe(
                unit => Console.WriteLine("Unit published"),
                () => Console.WriteLine("Action completed"));
        }
        private static void StartFunc()
        {
            var start = Observable.Start(() =>
                {
                    Console.Write("Working away");
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(100);
                        Console.Write(".");
                    }
                    return "Published value";
                });
            start.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Action completed"));
        }
        private static void ObservableWithInfiniteNaturalNumbers()
        {
            Console.WriteLine("Natural numbers");
            var numbers = Observable.Generate(0, i => true, i => i + 1, i => i);
            numbers.Subscribe(Console.WriteLine, () => Console.WriteLine("will never appear"));
        }

        private static void ObservableTime()
        {
            Console.WriteLine("test will appear after time of 5 seconds");
            var timer = Observable.Timer(TimeSpan.FromSeconds(5));
            timer.Subscribe(v => Console.WriteLine("test"), () => Console.WriteLine("completed"));
        }

        private static void ObservableInterval()
        {
            var interval = Observable.Interval(TimeSpan.FromSeconds(1));
            interval.Subscribe(v => Console.WriteLine(v % 2 == 0 ? "tik" : "tak"), () => Console.WriteLine("completed"));
            Console.ReadKey();

        }

        private static void ObservableRangeBuiltOnGenerate()
        {
            Range(11, 10).Subscribe(v => Console.WriteLine(v), () => Console.WriteLine("end"));
        }

        public static IObservable<int> Range(int start, int count)
        {
            var max = start + count;
            return Observable.Generate(
                start,
                value => value < max,
                value => value + 1,
                value => value);
        }


        private static void ObservableRange()
        {
            var range = Observable.Range(11, 10);
            range.Subscribe(v => Console.WriteLine(v), () => Console.WriteLine("end"));
        }

        private static void NonBlocking_event_driven()
        {
            var ob = Observable.Create<string>(
                observer =>
                    {
                        var timer = new System.Timers.Timer();
                        timer.Interval = 1000;
                        timer.Elapsed += (s, e) => observer.OnNext("tick");
                        timer.Elapsed += OnTimerElapsed;
                        timer.Start();
                        return () =>
                        {
                            timer.Elapsed -= OnTimerElapsed;
                            timer.Dispose();
                        };
                    });
            var subscription = ob.Subscribe(Console.WriteLine);
            Console.ReadLine();
            subscription.Dispose();
        }
        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime);
        }

        private static void SubjectUnsubscribing()
        {
            var values = new Subject<int>();
            var firstSubscription = values.Subscribe(value =>
                Console.WriteLine("1st subscription received {0}", value));
            var secondSubscription = values.Subscribe(value =>
                Console.WriteLine("2nd subscription received {0}", value));
            values.OnNext(0);
            values.OnNext(1);
            values.OnNext(2);
            values.OnNext(3);
            firstSubscription.Dispose();
            Console.WriteLine("Disposed of 1st subscription");
            values.OnNext(4);
            values.OnNext(5);
        }

        private static void SubjectSubscribeWithError()
        {
            var values = new Subject<int>();
            values.Subscribe(
                value => Console.WriteLine("1st subscription received {0}", value),
                ex => Console.WriteLine("Caught an exception : {0}", ex));
            values.OnNext(0);
            values.OnError(new Exception("Dummy exception"));
        }

        private static void SubjectInvalidUsageExample()
        {
            var subject = new Subject<string>();
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            subject.OnNext("c");
        }

        private static void AsyncSubjectWithCompleteSample()
        {
            var subject = new AsyncSubject<string>();
            subject.OnNext("a");
            WriteStreamToConsole(subject);
            subject.OnNext("b");
            subject.OnNext("c");
            subject.OnCompleted();
        }

        private static void AsyncSubjectSample()
        {
            var subject = new AsyncSubject<string>();
            subject.OnNext("a");
            WriteStreamToConsole(subject);
            subject.OnNext("b");
            subject.OnNext("c");
        }

        private static void BehaviorSubjectCompletedExample()
        {
            var subject = new BehaviorSubject<string>("a");
            subject.OnNext("b");
            subject.OnNext("c");
            subject.OnCompleted();//completed before subscribe was set
            subject.Subscribe(Console.WriteLine);
        }

        private static void BehaviorSubjectExample3()
        {
            var subject = new BehaviorSubject<string>("a");
            subject.OnNext("b");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("c");
            subject.OnNext("d");
        }

        private static void BehaviorSubjectExample2()
        {
            var subject = new BehaviorSubject<string>("a");
            subject.OnNext("b");
            subject.Subscribe(Console.WriteLine);
        }

        private static void BehaviorSubjectExample()
        {
            //Need to provide a default value.
            var subject = new BehaviorSubject<string>("a");
            subject.Subscribe(Console.WriteLine);
        }

        private static void ReplaySubjectWindowExample()
        {
            var window = TimeSpan.FromMilliseconds(150);

            var subject = new ReplaySubject<string>(window);
            subject.OnNext("w");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext("x");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext("y");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("z");
        }
        private static void ReplaySubjectBufferExample()
        {
            var bufferSize = 2;
            var subject = new ReplaySubject<string>(bufferSize);
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnNext("c");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("d");
        }
        private static void ReplaySubject()
        {
            var subject = new ReplaySubject<string>();
            subject.OnNext("a");
            WriteStreamToConsole(subject);
            subject.OnNext("b");
            subject.OnNext("c");
        }

        private static void SubjectSample1()
        {
            var subject = new Subject<string>();
            WriteStreamToConsole(subject);
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnNext("c");
        }

        static void WriteStreamToConsole(IObservable<string> stream)
        {
            //The next two lines are equivalent.
            //stream.Subscribe(value=>Console.WriteLine(value));
            stream.Subscribe(Console.WriteLine);

        }

        private static void ImplementingObserverAndObservable()
        {
            var numbers = new MySequenceOfNumbers();
            var observer = new MyConsoleObserver<int>();
            numbers.Subscribe(observer);
        }

        #region Anonymous Event
        public static event EventHandler MessageReceived;
        private static void RemoveAnonymousEventSample()
        {
            EventHandler action = (sender, e) =>
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(sender?.ToString());
                };

            MessageReceived += action;

            OnMessageReceived("test");

            // disposing
            MessageReceived -= action;
        }

        private static void OnMessageReceived(string msg)
        {
            MessageReceived?.Invoke(msg, EventArgs.Empty);
        }
        #endregion
    }

    public class MyConsoleObserver<T> : IObserver<T>
    {
        public void OnNext(T value)
        {
            Console.WriteLine("Received value {0}", value);
        }
        public void OnError(Exception error)
        {
            Console.WriteLine("Sequence faulted with {0}", error);
        }
        public void OnCompleted()
        {
            Console.WriteLine("Sequence terminated");
        }
    }

    public class MySequenceOfNumbers : IObservable<int>
    {
        public IDisposable Subscribe(IObserver<int> observer)
        {
            observer.OnNext(1);
            observer.OnNext(2);
            observer.OnNext(3);
            observer.OnCompleted();
            return Disposable.Empty;
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

        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return source.SelectMany(
                item =>
                    {
                        if (predicate(item))
                        {
                            return Observable.Return(item);
                        }
                        else
                        {
                            return Observable.Empty<T>();
                        }
                    });
        }
    }

    public class Person : IComparable
    {
        public string Name { get; set; }

        public int Age { get; set; }
        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"{Name}: {Age}";
        }

        public int CompareTo(object obj)
        {
            Person p = obj as Person;
            if (p == null) return 1;
            if (p.Age == Age) return 0;
            if (p.Age > Age) return -1;
            return 1;
        }
    }
}
