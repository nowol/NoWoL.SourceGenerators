using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        public void MainMethod()
        {
            TheMethod(() => SimulateWork(3000));
            TheMethod(() => SimulateWork(5000));
            AnotherMethod(SimulateWork);
            void TheMethod(System.Action funkyAsync)
            {
                funkyAsync();
            }

            void AnotherMethod(System.Action<int> funkyAsync)
            {
                funkyAsync(3);
            }

            int SimulateWork(int value)
            {
                System.Threading.Thread.Sleep(3000);
                return 3;
            }

            void TheMethodValue(System.Action funkyAsync)
            {
                funkyAsync();
            }

            void AnotherMethodValue(System.Action<int> funkyAsync)
            {
                funkyAsync(3);
            }

            int SimulateWorkValue(int value)
            {
                System.Threading.Thread.Sleep(3000);
                return 3;
            }

            void ReturnNonGenericTask(System.Action funkyAsync)
            {
                funkyAsync();
            }

            int ReturnGenericTask(System.Func<int> funkyZAsync)
            {
                return funkyZAsync();
            }

            int ReturnGenericTaskWhereTheReturnShouldBeKept(System.Func<int> funkyAsync)
            {
                return funkyAsync();
            }
        }
    }
}