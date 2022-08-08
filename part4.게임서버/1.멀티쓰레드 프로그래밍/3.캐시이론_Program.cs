using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {

        static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[y, x] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y,x)를 채우는데 걸린 시간 {end - now}");
            }

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[x, y] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(x,y)를 채우는데 걸린 시간 {end - now}");
            }

            // 위 코드 결과 아래쪽 코드의 시간이 더많이 걸린다.
            // for문의 순서를 보면 arr[y,x]는 [1,1]->[1,2]..순이고 arr[x,y]는 [1,1]->[2,1]순임을 알수있다.
            // 당연히 같은배열에서 두번째 칸을 보는것이, 새로운 배열을 찾는것보다 공간적으로 가까이 있을것이다.
            // 이게 캐시의 Spacial Locality다. (공간적 지역성=명령이 들어온곳 근처의 정보를 캐시에 담는다.)
        }


    }
}
 