using System;
using System.Collections.Generic;

namespace Tree
{
    // [ 트리 ]
    // 노드(Node)와 간선(Edge)을 통해 계층구조를 표현
    // 트리의 경우 노드의 삽입삭제가 많기에 그래프와 달리 실제 노드를 만들어주는게 좋다.
    class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();

    }

    class Program
    {
        static TreeNode<string> MakeTree()
        {
            //추가방식이 좀 독특한듯?
            TreeNode<string> root = new TreeNode<string>() { Data = "root" }; //트리의 root만듬
            {
                TreeNode<string> node = new TreeNode<string>() { Data = "1" };
                root.Children.Add(node);
                node.Children.Add(new TreeNode<string>() { Data = "1-1" });
                node.Children.Add(new TreeNode<string>() { Data = "1-2" });
                node.Children.Add(new TreeNode<string>() { Data = "1-3" });
                //node의 자식들은 leaf라서 새로 안만들었나?
            }
            {
                TreeNode<string> node = new TreeNode<string>() { Data = "2" }; //어쩌피 괄호가 다르니 윗괄호의 node와 이괄호의 node는 다른것이다
                root.Children.Add(node);
                node.Children.Add(new TreeNode<string>() { Data = "2-1" });
                node.Children.Add(new TreeNode<string>() { Data = "2-2" });
                node.Children.Add(new TreeNode<string>() { Data = "2-3" });
            }
            {
                TreeNode<string> node = new TreeNode<string>() { Data = "3" };
                root.Children.Add(node);
                node.Children.Add(new TreeNode<string>() { Data = "3-1" });
                node.Children.Add(new TreeNode<string>() { Data = "3-2" });
                node.Children.Add(new TreeNode<string>() { Data = "3-3" });
            } // 각 children마다 새로운 괄호를 펼쳐줬다는게 독특하네 {1}{2}{3}

            return root;
        }

        // 트리 기능들을 만들때, 서브트리 속성을 생각해 재귀함수로 만들면 쉽다.
        static void PrintTree(TreeNode<string> root) // 트리 순회
        {
            Console.WriteLine(root.Data); // 나를 프린트
            foreach (TreeNode<string> child in root.Children) // 내 모든 자식들에게
            {
                PrintTree(child); //자기 자신을 프린트하고 니 자식들에게도 알려라
            }
        }

        static int GetHeight(TreeNode<string> root) // 트리의 높이를 반환 // 코테문제로 나오기좋다.
        {
            int height = 0;
            foreach(TreeNode<string> child in root.Children)
            {
                int newHeight = GetHeight(child) + 1;
                if (height < newHeight) // if문으로 구별하지말고 Math.Max를 사용해도 된다.
                    height = newHeight;
                height = Math.Max(height, newHeight);//위 if문과 같은의미.
            }
                return height;
        }
        static void Main(string[] args)
        {
            TreeNode<string> root = MakeTree();
            PrintTree(root);
            Console.WriteLine(GetHeight(root));
        }
    }
}
