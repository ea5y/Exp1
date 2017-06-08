using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomControl
{
    #region Radar
    public class Radar
    {
        Vector2 point1;
        Vector2 point2;
        Vector2 point3;
        Vector2 point4;
        Vector2 point5;
        Vector2 origin;

        protected Vector3 _point1;
        protected Vector3 _point2;
        protected Vector3 _point3;
        protected Vector3 _point4;
        protected Vector3 _point5;

        private float _value1;//ctrl
        private float _value2;//atk
        private float _value3;//def
        private float _value4;//spd
        private float _value5;//aid

        private GameObject _radarObj;
        public MeshFilter _mf;
        public Material _mt;
        public MeshRenderer _mr;

        private Vector3[] newVertices;
        private Vector2[] newUV;
        private int[] newTriangles;

        public Radar(GameObject radar, Material material)
        {
            _radarObj = radar;
            _mt = material;

            this.SetPoint();

            if (_mf == null)
            {
                _mf = (MeshFilter)_radarObj.AddComponent(typeof(MeshFilter));
            }
            if (_mr == null)
            {
                _mr = (MeshRenderer)_radarObj.AddComponent(typeof(MeshRenderer));
                _mr.material = _mt;
            }
        }

        void SetPoint()
        {
            int scale = 200;
            point1 = new Vector2((float)-50 / scale, (float)12 / scale);
            point2 = new Vector2((float)0, (float)50 / scale);
            point3 = new Vector2((float)50 / scale, (float)12 / scale);
            point4 = new Vector2((float)30 / scale, (float)-50 / scale);
            point5 = new Vector2((float)-30 / scale, (float)-50 / scale);
            origin = new Vector2((float)0, (float)-8 / scale);
        }
        
        void Update()
        {
            _point1 = new Vector3(point1.x * _value1, point1.y * _value1, 0);
            _point2 = new Vector3(point2.x * _value2, point2.y * _value2, 0);
            _point3 = new Vector3(point3.x * _value3, point3.y * _value3, 0);
            _point4 = new Vector3(point4.x * _value4, point4.y * _value4, 0);
            _point5 = new Vector3(point5.x * _value5, point5.y * _value5, 0);

            newVertices = new Vector3[] { _point1, _point2, _point3, _point4, _point5, origin };

            newTriangles = new int[] { 5, 0, 1, 5, 1, 2, 5, 2, 3, 5, 3, 4, 5, 4, 0 };
            newUV = new Vector2[]{Vector2.Lerp (point1,origin,_value1),Vector2.Lerp (point2,origin,_value2),Vector2.Lerp (point3,origin,_value3),Vector2.Lerp (point4,origin,_value4)
        ,Vector2.Lerp (point5,origin,_value5),new Vector2(.5f,.5f)};


            _mf.mesh.vertices = newVertices;
            _mf.mesh.uv = newUV;
            _mf.mesh.triangles = newTriangles;
        }

        public void SetValues(float value1, float value2, float value3, float value4, float value5)
        {
            this._value1 = value1;
            this._value2 = value2;
            this._value3 = value3;
            this._value4 = value4;
            this._value5 = value5;
            this.Update();
        }
    }
    #endregion

    #region TabPages
    public interface IPage
    {
        GameObject Root { get; }
        void Init();
    }

    public interface IPageWithInit : IPage
    {
       
    }

    public class TabPagesManager
    {
        private List<IPage> pages = new List<IPage>();

        public TabPagesManager()
        {

        }

        public TabPagesManager(IPage[] pages)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                this.pages.Add(pages[i]);
            }
        }
        
        public void AddPage(IPage page)
        {
            this.pages.Add(page);
        }

        public void SwitchTo(IPage view)
        {
            foreach (var page in this.pages)
            {
                if (view == page)
                {
                    if (null != page.Root)
                    {
                        page.Root.SetActive(true);
                    }
                }
                else
                {
                    page.Init();
                    if (null != page.Root)
                    {
                        page.Root.SetActive(false);
                    }
                }
            }
        }
    }
    
    public interface IPageNew
    {
        GameObject Root { get; }
        System.Action Init { get; set; }
    }

    public class TabPagesManagerNew
    {
        private List<IPage> pages = new List<IPage>();

        public TabPagesManagerNew()
        {

        }

        public TabPagesManagerNew(IPage[] pages)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                this.pages.Add(pages[i]);
            }
        }
        
        public void AddPage(IPage page)
        {
            this.pages.Add(page);
        }

        public void SwitchTo(IPage view)
        {
            foreach (var page in this.pages)
            {
                if (view == page)
                {
                    if (null != page.Root)
                    {
                        page.Root.SetActive(true);
                    }
                }
                else
                {
                    page.Init();
                    if (null != page.Root)
                    {
                        page.Root.SetActive(false);
                    }
                }
            }
        }
    }
 
    #endregion
    
    #region ScrollView
    public class ScrollView<T> where T : ScrollViewItem
    {
        private UIScrollView _scrollView;
        private UIGrid _grid;
        private GameObject _itemPrefab;
        private UIPanel _panel;
        private Vector3 _startPos;
        private Vector3 lastPos;
        private Vector2 _clipOffset;
        private List<T> _scrollItems;
        private IList _datas;

        private Vector3 _savePos;
        private int cacheNum = 3; //多出来的缓存格子

        public ScrollView(UIGrid grid, GameObject itemPrefab)
        {
            _scrollView = grid.transform.parent.GetComponent<UIScrollView>();
            _grid = grid;
            _itemPrefab = itemPrefab;
            _panel = _scrollView.GetComponent<UIPanel>();
            _startPos = _panel.transform.localPosition;
            //最开始往下拉1个不重刷
            lastPos = new Vector3(0, _startPos.y + _grid.cellHeight, 0);
            _clipOffset = _panel.clipOffset;
            _savePos = _startPos;
            _scrollItems = new List<T>();
        }

        public void CreateWeight(IList datas)
        {
            _datas = datas;

            //Clear
            _grid.gameObject.DestroyChild();
            _scrollItems.Clear();

            int dataCount = datas.Count;//数据的数量
            int fillCount = 0; //当前scrollView被填满的格子数

            //Vector3 lastPos = Vector3.zero;
            //最开始往下拉1个不重刷
            //lastPos.y = _startPos.y + _grid.cellHeight;

            _panel.transform.localPosition = new Vector3( _startPos.x, _startPos.y, _startPos.z);
            _panel.clipOffset = _clipOffset;
            //Fit
            /*if(this._savePos != _startPos)
            {
                //
                Debug.Log("SavePos: " + this._savePos);
                SpringPanel.Begin(_panel.gameObject, this._savePos, 13f).strength = 8f;
            }else
            {
                SpringPanel.Begin(_panel.gameObject, this._startPos, 13f).strength = 8f;
            }*/

            SpringPanel.Begin(_panel.gameObject, this._startPos, 13f).strength = 8f;
            //Clac fillCount
            UIScrollView.Movement moveType = _scrollView.movement;
            if (moveType == UIScrollView.Movement.Vertical)
            {
                fillCount = Mathf.RoundToInt(_panel.height / _grid.cellHeight);
            }
            else if (moveType == UIScrollView.Movement.Horizontal)
            {
                fillCount = Mathf.RoundToInt(_panel.width / _grid.cellWidth);
            }

            // 面板没被占满拖拽回滚
            Debug.Log("dataCout: " + dataCount);
            Debug.Log("fillCount: " + fillCount);
            _scrollView.onMomentumMove = () => { };
            if (!_scrollView.disableDragIfFits)
            {                
                if (dataCount <= fillCount)
                {
                    Debug.Log("mianbanmeiman.");
                    lastPos = _panel.transform.localPosition;
                    _scrollView.onMomentumMove = () => { };
                    _scrollView.onMomentumMove = () => {
                        SpringPanel.Begin(_panel.gameObject, lastPos, 13f).strength = 8f;
                    };
                }
            }

            // 如果item数量大于填满显示面板的数量做优化
            if (dataCount > fillCount + cacheNum)
            {
                Debug.Log("单元数量大于面板显示数量。");
                dataCount = fillCount + cacheNum;
                //当前显示出来的第一个格子，在grid数据中的index
                int index;
                //上次显示出来的第一个格子，在grid数据中的index
                int lastIndex = 0; 
                int maxIndex = dataCount - 1;
                int minIndex = 0;
                
                _panel.onClipMove = (uiPanel) => {
                    //滑动距离
                    this._savePos = _panel.transform.localPosition;
                    Vector3 delata = lastPos - _panel.transform.localPosition;
                    float distance = delata.y != 0 ? delata.y : delata.x;
                    
                    // 满的时候向上滑不管它
                    if (distance > 0) return;

                    // 计算index
                    distance = -distance;
                    if (moveType == UIScrollView.Movement.Horizontal)
                    {
                        index = Mathf.FloorToInt(distance / _grid.cellWidth);
                    }
                    else
                    {
                        index = Mathf.FloorToInt(distance / _grid.cellHeight);
                    }

                    // 拖拽不满一个单元格
                    if (index == lastIndex)
                    {
                        //Debug.Log("拖拽不满一个单元。");
                        return;
                    }

                    // 拉到底了, 往回拉cacheNum-1-1个不重刷
                    if (index + dataCount > datas.Count)
                    {
                        //Debug.Log("到底了。");
                        return;
                    }
                    
                    // 重刷
                    int offset = Math.Abs(index - lastIndex);
                    // 判断要把最上（左）的item移动到最下（右）,还是相反
                    //Debug.Log("lastIndex: " + lastIndex);
                    //Debug.Log("index: " + index);
                    if (lastIndex < index)
                    {
                        //Debug.Log("TopToBottom.");
                        //Debug.Log("Offset: " + offset);
                        for (int i = 1; i <= offset; i++)
                        {
                            //Debug.Log("Go!");
                            //上（左）移动到下（右）
                            int curIndex = maxIndex + 1;
                            T item = _scrollItems[0];
                            _scrollItems.Remove(item);
                            _scrollItems.Add(item);
                            item.FillItem(datas, curIndex);
                            minIndex++;
                            maxIndex++;
                        }
                    }
                    else
                    {
                        //Debug.Log("BottomToTop.");
                        //Debug.Log("Offset: " + offset);
                        for (int i = 1; i <= offset; i++)
                        {
                            //Debug.Log("Go!");
                            int curIndex = minIndex - 1;
                            T item = _scrollItems[_scrollItems.Count - 1];
                            _scrollItems.Remove(item);
                            _scrollItems.Insert(0, item);
                            item.FillItem(datas, curIndex);
                            minIndex--;
                            maxIndex--;
                        }
                    }
                    lastIndex = index;
                };
            }

            
            // 添加能填满UI数量的button
            for (int i = 0; i < dataCount; i++)
            {
                GameObject go = NGUITools.AddChild(_grid.gameObject, _itemPrefab);
                go.SetActive(true);

                T item = go.GetComponent<T>();
                if (item == null)
                {
                    item = go.AddComponent<T>();
                }
                    
                item.Grid = _grid;
                _scrollItems.Add(item);
                item.FillItem(datas, i);
            }
            
            //_grid.Reposition();
        }

        public void CreateLightWeight(IList datas)
        {
            _datas = datas;
            int len = _scrollItems.Count > _datas.Count ? _scrollItems.Count : _datas.Count;
            for(int i = 0; i < len; ++i)
            {
                if(i >= _scrollItems.Count)
                {
                    var go = NGUITools.AddChild(_grid.gameObject, _itemPrefab);
                    T item = go.GetComponent<T>();
                    if (item == null)
                        item = go.AddComponent<T>();
                    _scrollItems.Add(item);
                }
                if (i < _datas.Count)
                {
                    _scrollItems[i].gameObject.SetActive(true);
                    _scrollItems[i].FillItem(datas, i);
                }
                else
                    _scrollItems[i].gameObject.SetActive(false);
            }
            _grid.Reposition();
        }

        public K FindCellData<K>(int index) where K : ScrollViewCellItemData
        {

            for(int j = 0; j < _datas.Count; j++)
            {
                var cellDataList = (List<K>)_datas[j];
                for (int i = 0; i < cellDataList.Count; i++)
                {
                    if (cellDataList[i].index == index)
                    {
                        //Finded
                        Debug.Log("Data Finded.");
                        return cellDataList[i];
                    }
                }
            }
            return null;
        }

        public Y FindCellItem<Y>(int index) where Y : ScrollViewCellItem
        {
            for(int i = 0; i < _scrollItems.Count; i++)
            {
                var cellItemList = (List<Y>)_scrollItems[i].GetCellItemList();
                for(int j = 0; j < cellItemList.Count; j++)
                {
                    if(cellItemList[j].index == index)
                    {
                        //Finded
                        Debug.Log("Item Finded.");
                        return cellItemList[j];
                    }
                }
            }

            return null;
        }

        public void FindCellItemAndChange<K,Y>(int index, Action<K, Y> callback) where K : ScrollViewCellItemData where Y : ScrollViewCellItem 
        {
            var data = this.FindCellData<K>(index);
            var item = this.FindCellItem<Y>(index);

            callback(data, item);
        }

        public void SetCachNum(int n)
        {
            cacheNum = n;
        }

        public void SetDragFillDelay(int n)
        {
            this.lastPos = new Vector3(0, _startPos.y + _grid.cellHeight * 0, 0);
        }
    }
    
    public class ScrollPage<T> where T: ScrollView3DItem
    {
        int ITEM_NUM = 0;//Page num
        const float DRAG_SPEED = 0.5f;//Tween play time
        const int DRAG_OFFSET = 30;//Drag distance

        float beganX = 0;
        float beganY = 0; //Start pos

        float currentIndex = 0;
        bool isPlay = false; //Is tween playing
        bool isPress = false;
        bool isPageFoot = false;
        bool isPageHome = true;

        GameObject container;
        GameObject templateItem;
        IList datas;
        List<T> scrollItems;
        public int counter = 0;
        int half = 0;
        public int movX = 70;
        int movY = 0;
        int movZ = 0;

        public ScrollPage(GameObject container)
        {
            this.container = container;
            this.scrollItems = new List<T>();
        }

        public IEnumerator Create(GameObject prefab, IList datas)
        {
            this.datas = datas;
            this.templateItem = prefab;
            this.ITEM_NUM = datas.Count;

            //NGUITools.DestroyChildren(container.transform);
            container.DestroyChildImmediate();
            this.scrollItems.Clear();           

            yield return this._CreateThenSelect();
        }

        IEnumerator _CreateThenSelect()
        {
            for (int i = 0; i < this.datas.Count; i++)
            {
                GameObject go = NGUITools.AddChild(this.container, this.templateItem);
                go.SetActive(true);
                T item = go.GetComponent<T>();
                
                go.transform.GetChild(0).gameObject.AddComponent<UIEventListener>();
                UIEventListener.Get(go.transform.GetChild(0).gameObject).onPress = this.OnPressEvent;

                this.scrollItems.Add(item);
                item.FillItem(this.datas, i);
            }

            while (this.counter != this.datas.Count)
                yield return null;

            Debug.Log("Counter: " + this.counter);
            this.counter = 0;
        }

        //no loop
        public void SelectHalf(int half)
        {
            if (half < 0 || half > this.datas.Count - 1)
                return;
            this.half = half;            
            
            foreach (T role in this.scrollItems)
            {
                int index = role.Index;
                role.Half = this.half;
                this.SetDepthAndPositionNonLoop(role, index);
            }
        }

        public void Scale(T role)
        {
            TweenScale ts = role.GetComponent<TweenScale>();
            if (role.Index == this.half)
                ts.PlayForward();
            else
                ts.PlayReverse();
        }


        private void SetDepthAndPositionNonLoop(T role, int index)
        {
            TweenPosition tp = role.GetComponent<TweenPosition>();

            int offSet = this.half - index;

            int x = offSet * this.movX;
            int y = offSet < 0 ? -offSet * this.movY : offSet * this.movY;
            int z = offSet < 0 ? -offSet * this.movZ : offSet * this.movZ;
            tp.to = new Vector3(x, y, z);

            for (int i = 0; i < role.widgets.Count; i++)
            {
                role.widgets[i].depth = role.widgets.Count * ((offSet < 0 ? -1 : 1) * index) + i;
            }
            
            tp.PlayForward();
            this.Scale(role);
        }

        public List<T> GetScrollItems()
        {
            return this.scrollItems;
        }
        
        void OnPressEvent(GameObject obj, bool isDown)
        {
            Debug.Log("Press.");
            float endX;
            float endY;
            if (isDown)
            {
                beganX = UICamera.lastEventPosition.x;
                beganY = UICamera.lastEventPosition.y;
                isPress = true;
            }
            else
            {
                Debug.Log("Release.");
                endX = UICamera.lastEventPosition.x;
                endY = UICamera.lastEventPosition.y;
                if (isPress)
                {
                    if (isPlay == false)
                    {
                        Debug.Log("End - Began: " + (endX - beganX));
                        if (Math.Abs(endX - beganX) > DRAG_OFFSET && Math.Sign(endX - beganX) > 0)
                        {
                            this.SelectHalf(this.half + 1);
                        }
                        else if (Math.Abs(endX - beganX) > DRAG_OFFSET && Math.Sign(endX - beganX) < 0)
                        {
                            this.SelectHalf(this.half - 1);
                        }
                    }
                }
                isPress = false;
            }
        }        
    }
    
    public class ScrollView3DItem : ScrollViewItem
    {
        int _half = -1;
        public int Half {
            get { return _half; }

            set
            {
                _half = value;
            }
        }
        public List<UIWidget> widgets;

        public virtual void Finish()
        {
            TweenPosition tp = gameObject.GetComponent<TweenPosition>();
            tp.from = gameObject.transform.localPosition;
            tp.ResetToBeginning();
            if(this.Half == this.Index)
            {
                this.Select();
            }
        }

        public virtual void Select()
        {
           
        }        
    }

    public class ScrollViewCellItemData
    {
        public int index;
    }

    public class ScrollViewCellItem : MonoBehaviour
    {
        public int index;
    }

    public class ScrollViewItem : MonoBehaviour
    {
        protected IList cellItemList;

        private int _index;
        public int Index
        {
            get { return _index; }
            protected set { _index = value; }
        }
        
        private UIScrollView.Movement _moveType;
        private UIScrollView _scrollView;
        private UIGrid _grid;
        public UIGrid Grid
        {
            set
            {
                _grid = value;
                _scrollView = _grid.transform.parent.GetComponent<UIScrollView>();
                _moveType = _scrollView.movement;
            }
            get { return _grid; }
        }

        public virtual void FillItem(IList datas, int index)
        {
            this.Index = index;
            if (_moveType == UIScrollView.Movement.Horizontal)
            {
                transform.localPosition = new Vector3(-this.Grid.cellWidth * index, 0, 0);
            }
            else if (_moveType == UIScrollView.Movement.Vertical)
            {
                transform.localPosition = new Vector3(0, -this.Grid.cellHeight * index, 0);
            }
        }

        public IList GetCellItemList()
        {
            return this.cellItemList;
        }
    }
    #endregion

    #region SliderTool
    public class SliderTool
    {
        public List<List<SliderData>> StepBuffer { get; private set; }
        public Queue<List<SliderData>> StepQueue { get; private set; }
        public float MaxAmount { get; set; }
        public bool Enable { get; set; }

        private bool isUp = true;
        public bool IsUp
        {
            get { return this.isUp; }
            set { this.isUp = value; }
        }
        
        private bool isDefultSpeed = true;
        public bool IsDefultSpeed
        {
            get { return this.isDefultSpeed; }
            set { this.isDefultSpeed = value; }
        }

        private float speed;
        public float Speed
        {
            get { return this.speed; }
            set
            {
                this.speed = value;
                this.IsDefultSpeed = false;
            }
        }

        private int counter = 0;

        public SliderTool(float maxAmount)
        {
            this.MaxAmount = maxAmount;
            this.StepBuffer = new List<List<SliderData>>();
            this.StepQueue = new Queue<List<SliderData>>();
        }

        public void FillStepBuffer(SliderData data)
        {
            if (data != null)
            {
                this.CreateStep(data);
                FillStepBuffer(data.data);
            }
        }

        private void CreateStep(SliderData data)
        {
            if (this.StepBuffer.Count - 1 < data.index)
            {
                this.StepBuffer.Add(new List<SliderData>());
            }

            if (data.hook)
                this.StepBuffer[data.index].Add(data);
        }

        public void AddToQueue()
        {
            //add to queue
            foreach (var step in this.StepBuffer)
            {
                this.StepQueue.Enqueue(step);
            }
        }

        public void Up(SliderData data)
        {
            if (data.amount < data.totalAmount && data.amount != -1)
            {
                if (this.IsDefultSpeed)
                    data.amount += (Time.deltaTime * this.CalcSpeed(data.index));
                else
                    data.amount += (Time.deltaTime * this.Speed);
                data.slider.fillAmount = data.amount;

                if (data.amount >= this.MaxAmount)
                    data.slider.fillAmount = this.MaxAmount;

                if (data.amount >= data.totalAmount)
                {
                    counter++;
                    Debug.Log("Counter: " + counter);
                }

                if (data.amount >= this.MaxAmount)
                {
                    if (data.lblLv != null)
                        data.lblLv.text = data.nextLv + "";
                    if (data.imgLv != null)
                        data.imgLv.spriteName = data.nextLv + "";
                }

            }
        }

        private void Down(SliderData data)
        {
            if(data.amount > data.totalAmount)
            {
                if (this.IsDefultSpeed)
                    data.amount -= (Time.deltaTime * this.CalcSpeed(data.index));
                else
                    data.amount -= (Time.deltaTime * this.Speed);
                data.slider.fillAmount = data.amount;

                if (data.amount <= 0)
                    data.slider.fillAmount = 0;

                if (data.amount <= data.totalAmount)
                {
                    counter++;
                    Debug.Log("Counter: " + counter);
                }

                if(data.amount <= 0)
                {
                    if (data.lblLv != null)
                        data.lblLv.text = data.nextLv + "";
                    if (data.imgLv != null)
                        data.imgLv.spriteName = data.nextLv + "";
                }
            }
        }

        private float CalcSpeed(int index)
        {
            //y=x+1
            var speed = 0.2f * (-index + this.StepBuffer.Count - 1) + 0.2f;
            return speed;
        }

        public void Update()
        {
            if (this.Enable)
            {
                if (this.StepQueue.Count != 0)
                {
                    var step = this.StepQueue.Peek();
                    foreach (var data in step)
                    {
                        if (this.IsUp)
                            this.Up(data);
                        else
                            this.Down(data);
                    }
                    if (this.counter >= step.Count)
                    {
                        this.counter = 0;
                        this.StepQueue.Dequeue();
                    }
                }
                else
                {
                    this.Enable = false;
                }
            }
        }
    }

    public class SliderData
    {
        public int index;        
        public bool hook;//if true i need, else discard

        public UISprite slider;
        public UILabel lblLv;
        public UISprite imgLv;

        public int nextLv;
        public float amount;
        public float residueExp;
        public float curExpAfterAdd;
        public float totalAmount;

        public SliderData data;
    }
    #endregion

    #region Tool Func
    public class ToolFunc
    {
        #region TabMenu
        public static void BtnSwitchTo(UIButton btnSel, List<UIButton> btnList)
        {
            foreach (UIButton btn in btnList)
            {
                if (btnSel == btn)
                {
                    btn.enabled = false;
                    btn.SetState(UIButton.State.Disabled, true); //btn.disabledSprite
                    continue;
                }
                if (btn.enabled == false)
                    btn.enabled = true;
            }
        }
        #endregion

        #region Time
        public static string TimeSecondIntToString(long second, bool isHour)
        {
            string str = "";
            long hour, min, sec;
            try
            {
                if (!isHour)
                {
                    min = second / 60;
                    sec = second % 60;
                    if (min < 10)
                        str += "0" + min;
                    else
                        str += "" + min;
                    str += ":";
                    if (sec < 10)
                        str += "0" + sec;
                    else
                        str += "" + sec;
                    //return str;
                }
                else
                {
                    hour = second / 3600;
                    min = second % 3600 / 60;
                    sec = second % 60;
                    if (hour < 10)
                    {
                        str += "0" + hour.ToString();
                    }
                    else
                    {
                        str += hour.ToString();
                    }
                    str += ":";
                    if (min < 10)
                    {
                        str += "0" + min.ToString();
                    }
                    else
                    {
                        str += min.ToString();
                    }
                    str += ":";
                    if (sec < 10)
                    {
                        str += "0" + sec.ToString();
                    }
                    else
                    {
                        str += sec.ToString();
                    }
                    //return str;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Catch:" + ex.Message);
            }
            return str;
        }

        public static string TimeCountDown(long second)
        {
            string str = "";
            float day, hour, min, sec;

            day = (float)second / (3600 * 24);
            hour = (float)second %(3600 * 24) / 3600;
            min = (float)second %(3600 * 24) % 3600 / 60;
            sec = (float)second % 60;

            if(day >= 1)
            {
                str += Mathf.Ceil(day) + "天";
            }
            else if(hour >= 1)
            {
                str += Mathf.Ceil(hour) + "小时";
            }
            else if(min >= 1)
            {
                str += Mathf.Ceil(min) + "分";
            }
            else if(sec >= 1)
            {
                str += sec + "秒";
            }
            return str;
        }
        #endregion

        #region GridResponse
        public static void GridAddItemAndResponse(GameObject group, GameObject item, int cellWidth, int childCount)
        {
            int sumWidth = cellWidth * childCount;
            var startPos = -(sumWidth / 2) + (cellWidth / 2);

            group.DestroyChildImmediate();
            for(int i = 0; i < childCount; i++)
            {
                var go = NGUITools.AddChild(group, item);
                go.SetActive(true);
                go.transform.localPosition = new Vector3(startPos + i * cellWidth, 0, 0);
            }
        }

        public static void GridFillItem<T>(GameObject gridGroup, IList datas) where T : IGridItem
        {
            for(int i = 0; i < datas.Count; i++)
            {
                var go = gridGroup.transform.GetChild(i);
                T item = go.GetComponent<T>();
                item.Fill(datas[i]);
            }
        }

        public static void GridFillItem<T>(GameObject gridGroup, object data) where T : IGridItem
        {
            
            var go = gridGroup.transform.GetChild(0);
            T item = go.GetComponent<T>();
            item.Fill(data);
        }

        public static void ResponseGrid(GameObject gridGroup, int cellWidth)
        {
            int sumWidth = cellWidth * gridGroup.transform.childCount;
            var startPos = -(sumWidth / 2) + (cellWidth / 2);

            int counter = 0;
            foreach(Transform t in gridGroup.transform)
            {
                t.localPosition = new Vector3(startPos + counter * cellWidth, 0, 0); 
                counter++;
            }
        }

        public interface IGridItem
        {
            void Fill(object data);
        }

        #endregion
    }
    #endregion


}
