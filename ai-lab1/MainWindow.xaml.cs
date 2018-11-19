using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Immutable;



namespace ai_lab1
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int StateSize = 19;

        enum Item { Simple = 0, Red = 1}; // Тип кружочка
        enum Action: int { a0, a1, a2, a3, a4, a5, a6 , none}; // Возможные ходы - повороты вокруг 0..6 кружка

        // chains - цепочка кружков для поворота
        ImmutableArray<ImmutableArray<int>> chains = ImmutableArray.Create(new ImmutableArray<int>[] {
            ImmutableArray.Create( new int[] { 1, 2, 3, 4, 5, 6 } ),
            ImmutableArray.Create( new int[] { 7, 8, 2, 0, 6, 18 } ),
            ImmutableArray.Create( new int[] { 8, 9, 10, 3, 0, 1 } ),
            ImmutableArray.Create( new int[] { 2, 10, 11, 12, 4, 0 } ),
            ImmutableArray.Create( new int[] { 0, 3, 12, 13, 14, 5 } ),
            ImmutableArray.Create( new int[] { 6, 0, 4, 14, 15, 16 } ),
            ImmutableArray.Create( new int[] { 18, 1, 0, 5, 16, 17 } )
        });

        //int[,] chains = new int[7, 6] { { 1, 2, 3, 4, 5 , 6},
        //    { 7, 8, 2, 0, 6, 18}, { 8, 9, 10, 3, 0, 1 }, { 2, 10, 11, 12, 4, 0 }, 
        //    { 6, 0, 4, 14, 15, 16 }, { 6, 0, 4, 14, 15, 16 }, { 18, 1, 0, 5, 16, 17 }
        //};

        // Item[position] - состояние кружочка в i-позиции
        // Item[] - полное состояние игры
        Item[] state0 = new Item[StateSize] { Item.Simple,
            Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple,
            Item.Simple, Item.Simple, Item.Red, Item.Red,    Item.Simple, Item.Simple,
            Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple };
        //
        Item[] state_target = new Item[StateSize] { Item.Simple,
            Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple,
            Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple, Item.Simple,
            Item.Simple, Item.Simple, Item.Red, Item.Red, Item.Simple, Item.Simple };
        //
        Button[] buttons_main = new Button[StateSize];
        Button[] buttons_target = new Button[StateSize];
        //

        // ==========================================================================================================
        public MainWindow()
        {
            InitializeComponent();
            // Привязка визуальных компонентов в массиив:
            for (int i = 0; i < StateSize; i++)
            {
                Button btn_main = (Button)CanvasMain.FindName("btn_" + i);
                if (btn_main != null)
                {
                    buttons_main[i] = btn_main;
                    btn_main.Click += ButtonZ_Click;
                    btn_main.Tag = i;
                }
                Button btn_target = (Button)CanvasTarget.FindName("btn_" + (i + 100));
                if (btn_target != null)
                {
                    buttons_target[i] = btn_target;
                    btn_target.Tag = i;
                }
            }

            DrawState(state0);
            DrawTargetState(state_target);
        }

        /// <summary>
        /// Функция проверки эквивалентности двух состояний
        /// </summary>
        /// <param name="state1"></param>
        /// <param name="state2"></param>
        /// <returns></returns>
        private bool StatesEquals(Item[] state1, Item[] state2)
        {
            if (state1.Length != state2.Length)
                return false;
            for (int i = 0; i < state1.Length; i++)
                if (state1[i] != state2[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Функция возвращает новое состояние полученное из начального initialState при помощи действия action,
        /// т.е. поворота вокруг точки Action.x
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private Item[] NextState(Item[] initialState, Action action)
        {
            Item[] finalState = new Item[initialState.Length]; // Создаем новое состояние
            Array.Copy(initialState, finalState, initialState.Length); // Копируем в него начальное
            Item buffer = finalState[chains[(int)action][5]]; // Состояние 5-го итема запоминаем
            for (int i = 5; i > 0; i--) // сдвигаем все значения от 1-го до 5-го
                finalState[chains[(int)action][i]] = finalState[chains[(int)action][i-1]];
            finalState[chains[(int)action][0]] = buffer; // в 0-й записываем сохраненное значение
            return finalState;
        }

        // ==========================================================================================================
        private void Log(string msg) { log.Text += msg + Environment.NewLine; }

        // ==========================================================================================================
        private void LogState(Item[] state)
        {
            for (int i = 0; i < state.Length; i++)
                log.Text += i + "=" + state[i] + " ";
            log.Text += Environment.NewLine;
        }

        // ==========================================================================================================
        private void DrawState(Item[] state)
        {
            // На входе - регион отображения или ?
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] == Item.Simple)
                    buttons_main[i].Template = (ControlTemplate)Application.Current.Resources["ButtonTemplate0"];
                if (state[i] == Item.Red)
                    buttons_main[i].Template = (ControlTemplate)Application.Current.Resources["ButtonTemplate1"];
            }
        }

        // ==========================================================================================================
        private void DrawTargetState(Item[] state)
        {
            // На входе - регион отображения или ?
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] == Item.Simple)
                    buttons_target[i].Template = (ControlTemplate)Application.Current.Resources["ButtonTemplate0"];
                if (state[i] == Item.Red)
                    buttons_target[i].Template = (ControlTemplate)Application.Current.Resources["ButtonTemplate1"];
            }
        }

        // ==========================================================================================================
        // ==========================================================================================================
        class Node : IEquatable<Node>
        {
            private Item[] _state;
            Node _parent;
            Action _action;

            public Item[] state { get => _state; }
            public Node parent { get => _parent; }
            public Action action { get => _action; }

            public Node(Node ParentNode, Item[] State, Action Action, ImmutableArray<ImmutableArray<int>> chains)
            {
                _action = Action;
                _parent = ParentNode;
                if (_parent != null)
                {
                    _state = new Item[_parent._state.Length]; // Создаем новое состояние
                    Array.Copy(_parent._state, _state, _parent._state.Length); // Копируем в него начальное
                    Item buffer = _state[chains[(int)action][5]]; // Состояние 5-го итема запоминаем
                    for (int i = 5; i > 0; i--) // сдвигаем все значения от 1-го до 5-го
                        _state[chains[(int)action][i]] = _state[chains[(int)action][i - 1]];
                    _state[chains[(int)action][0]] = buffer; // в 0-й записываем сохраненное значение
                }
                else
                    _state = State;
            }

            private bool StatesEquals(Item[] state1, Item[] state2)
            {
                if (state1.Length != state2.Length)
                    return false;
                for (int i = 0; i < state1.Length; i++)
                    if (state1[i] != state2[i])
                        return false;
                return true;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                Node objAsNode = obj as Node;
                if (objAsNode == null) return false;
                else return Equals(objAsNode);
            }

            public bool Equals(Node other)
            {
                if (other == null) return false;
                return StatesEquals(this._state, other._state);
            }
        }
        // ==========================================================================================================

        // ==========================================================================================================
        private Node BFSsearch(Item[] start_state)
        {
            Queue<Node> Olist = new Queue<Node>(); // Список открытых вершин
            Queue<Node> Clist = new Queue<Node>(); // Список закрытых вершин
            var xNode = new Node(null, start_state, Action.none, chains); // Вершина с начальным состоянием
            Olist.Enqueue(xNode); // Поместили в список начальное состояние

            while (Olist.Count > 0)
            {
                xNode = Olist.Dequeue();
                if (StatesEquals(xNode.state, state_target)) return xNode; // Если текущее состояние совпадает с целью, то все, на выход!
                Clist.Enqueue(xNode); // Помещаем текущйю вершину в закрытый список
                // Раскрываем текущую вершину:                                      
                for (int i = 0; i < 7; i++)
                {
                    var iNode = new Node(xNode, null, (Action)i, chains); // Вершина i кандидат
                    if (!StatesEquals(xNode.state, iNode.state)) // Если новое состояние отличается от текущего, то
                        if (!Olist.Contains(iNode) && !Clist.Contains(iNode)) // Если кандидата нет в закрытом и открытом списке, то
                            Olist.Enqueue(iNode); // помещяем кандидата в открытый список
                }
            }
            // Если мы здесь, то решения нет:
            return null;
        }

        // ==========================================================================================================
        private Node DFSsearch(Item[] start_state)
        {
            Stack<Node> Olist = new Stack<Node>(); // Список открытых вершин
            Stack<Node> Clist = new Stack<Node>(); // Список закрытых вершин
            var xNode = new Node(null, start_state, Action.none, chains); // Вершина с начальным состоянием
            Olist.Push(xNode); // Поместили в список начальное состояние

            while (Olist.Count > 0)
            {
                xNode = Olist.Pop();
                if (StatesEquals(xNode.state, state_target)) return xNode; // Если текущее состояние совпадает с целью, то все, на выход!
                Clist.Push(xNode); // Помещаем текущйю вершину в закрытый список
                // Раскрываем текущую вершину:                                      
                for (int i = 0; i < 7; i++)
                {
                    var iNode = new Node(xNode, null, (Action)i, chains); // Вершина i кандидат
                    if (!StatesEquals(xNode.state, iNode.state)) // Если новое состояние отличается от текущего, то
                        if (!Olist.Contains(iNode) && !Clist.Contains(iNode)) // Если кандидата нет в закрытом и открытом списке, то
                            Olist.Push(iNode); // помещяем кандидата в открытый список
                }
            }
            // Если мы здесь, то решения нет:
            return null;
        }

        // ==========================================================================================================
        // ==========================================================================================================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Ellipse eee = (Ellipse)btn0.Template.FindName("buttonSurface", btn0);
            eee.Fill = Brushes.Black;
            btnONE.Content = chains[0][2];
            LogState(state0);
            LogState(NextState(state0, Action.a2));
            DrawState(state0);
        }

        private void ButtonZ_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType().Name == "Button")
            {
                Button btn = (Button)sender;
                if (Int32.Parse(btn.Tag.ToString()) > 6) return;
                Action action = (Action)Int32.Parse(btn.Tag.ToString());
                state0 = NextState(state0, action);
                Log(action.ToString());
                DrawState(state0);
                if (StatesEquals(state0, state_target))
                    Log("VICTORY!!!!");
            }
        }

        private void btnSearch1_Click(object sender, RoutedEventArgs e)
        {
            Node node = BFSsearch(state0);
            if (node != null)
            {
                Log("===============================");
                Log("Решение найдено:");
                var actions = new List<Action>();
                var iNode = node;
                while (iNode.parent != null)
                {
                    actions.Add(iNode.action);
                    iNode = iNode.parent;
                }
                Log(String.Format("Количество шагов: {0}", actions.Count));
                actions.Reverse();
                foreach (var a in actions)
                    Log(a.ToString());
                Log("===============================");
            }
        }

        private void btnSearch2_Click(object sender, RoutedEventArgs e)
        {
            Node node = DFSsearch(state0);
            if (node != null)
            {
                Log("===============================");
                Log("Решение найдено:");
                var actions = new List<Action>();
                var iNode = node;
                while (iNode.parent != null)
                {
                    actions.Add(iNode.action);
                    iNode = iNode.parent;
                }
                Log(String.Format("Количество шагов: {0}", actions.Count));
                actions.Reverse();
                foreach (var a in actions)
                    Log(a.ToString());
                Log("===============================");
            }
        }
    }
}
