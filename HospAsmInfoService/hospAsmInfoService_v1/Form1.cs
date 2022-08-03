using System.Xml;
using System.Collections.Concurrent;
using System.Web;

namespace hospAsmInfoService_v1
{
    public partial class Form1 : Form
    {
        private const string GET_URL = "http://apis.data.go.kr/B551182/hospAsmInfoService/getHospAsmInfo?serviceKey={0}&pageNo={1}&numOfRows={2}&ykiho=JDQ4MTg4MSM1MSMkMSMkMCMkODkkMzgxMzUxIzExIyQxIyQzIyQ4OSQyNjE4MzIjNTEjJDEjJDYjJDgz";
        private const string SERVICEKEY = "����Ű";
        private const int PAGERESULTCONUT = 1000;
        private readonly HttpClient httpClient = new();
        private CancellationTokenSource? cancellationTokenSource;
        ConcurrentQueue<HospitalInfo> hospitalInfoQueue = new();
        private int pageNo = 1;

        public Form1()
        {
            InitializeComponent();
            SetControlEvent();
        }

        private void SetControlEvent()
        {
            this.Load += Form1_Load;
            lv_HospitalList.ItemSelectionChanged += Lv_HospitalList_ItemSelectionChanged;
        }

        private void Lv_HospitalList_ItemSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.ItemIndex >= 0)
            {
                var hospitalInfo = (HospitalInfo)lv_HospitalList.Items[e.ItemIndex].Tag;
                var encodingUrl = HttpUtility.UrlEncode(hospitalInfo.Addr, System.Text.Encoding.UTF8);
                string navigateUrl = $"http://maps.google.com/maps?q={encodingUrl}&t=m&z=0";
                wv2_Location.Source = new(navigateUrl);
            }
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            SetUI();
            cancellationTokenSource = new();
            AddListViewHospitalList(cancellationTokenSource.Token);
            await RequestHospitalList(cancellationTokenSource.Token);
        }

        private void SetUI()
        {
            lv_HospitalList.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(lv_HospitalList, true, null);
            lv_HospitalList.BeginUpdate();
            lv_HospitalList.GridLines = true;
            lv_HospitalList.View = View.Details;
            lv_HospitalList.Items.Clear();
            lv_HospitalList.Columns.Add("����", 100, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�����ڵ��", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�ּ�", 150, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�޼��������", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("������", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("��������", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�Ƿ�޿����Ű�", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�������� �������� �׻���", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("���󵿸ƿ�ȸ��", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�޼���⵵���� �׻��� ó���", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�ֻ��� ó���", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("��ǰ���", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("��纴��", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�索��", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�����", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("����", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�����", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("���", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("õ��", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("������⼺����ȯ", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("���", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("��ȯ�ڽ�", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�Ż�����ȯ�ڽ�", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("����", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("���Űǰ� �Կ�����", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("�޼��ϱ⵵���� �׻��� ó���", 50, HorizontalAlignment.Center);
            lv_HospitalList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lv_HospitalList.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv_HospitalList.EndUpdate();
        }

        private async Task RequestHospitalList(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string getRequestUrl = string.Format(GET_URL, SERVICEKEY, $"{pageNo}", $"{PAGERESULTCONUT}");

                    using var responseStream = await httpClient.GetStreamAsync(getRequestUrl, cancellationToken);

                    var xmlDocument1 = new XmlDocument();
                    xmlDocument1.Load(responseStream);

                    using XmlNodeList xmlNodeHeaderList = xmlDocument1.GetElementsByTagName("header");
                    if (xmlNodeHeaderList.Count > 0)
                    {
                        /*
                         �����ڵ�	����޽���	    ����
                               00   NORMAL SERVICE. ����
                         */
                        string resultCode = xmlNodeHeaderList[0]["resultCode"].InnerText;
                        string resultMsg = xmlNodeHeaderList[0]["resultMsg"].InnerText;

                        if (resultCode.Equals("00"))
                        {
                            using XmlNodeList xmlNodeBodyrList = xmlDocument1.GetElementsByTagName("body");

                            pageNo = Convert.ToInt32(xmlNodeBodyrList[0]["pageNo"].InnerText);
                            pageNo++;
                            string totalCount = xmlNodeBodyrList[0]["totalCount"].InnerText;
                            this.Invoke(() => { this.Text = $"totalCount : {totalCount} ��ȸ ����"; });
                            
                            using XmlNodeList xmlNodeItemList = xmlDocument1.GetElementsByTagName("item");
                            if (xmlNodeItemList.Count > 0)
                            {
                                foreach (XmlNode xmlNode in xmlNodeItemList)
                                {
                                    HospitalInfo hospitalInfo = new();
                                    hospitalInfo.Ykiho = xmlNode["ykiho"].InnerText;
                                    hospitalInfo.YadmNm = xmlNode["yadmNm"].InnerText;
                                    hospitalInfo.ClCd = xmlNode["clCd"].InnerText;
                                    hospitalInfo.ClCdNm = xmlNode["clCdNm"].InnerText;
                                    hospitalInfo.Addr = xmlNode["addr"].InnerText;
                                    hospitalInfo.AsmGrd01 = xmlNode["asmGrd01"]?.InnerText;
                                    hospitalInfo.AsmGrd02 = xmlNode["asmGrd02"]?.InnerText;
                                    hospitalInfo.AsmGrd03 = xmlNode["asmGrd03"]?.InnerText;
                                    hospitalInfo.AsmGrd04 = xmlNode["asmGrd04"]?.InnerText;
                                    hospitalInfo.AsmGrd05 = xmlNode["asmGrd05"]?.InnerText;
                                    hospitalInfo.AsmGrd06 = xmlNode["asmGrd06"]?.InnerText;
                                    hospitalInfo.AsmGrd07 = xmlNode["asmGrd07"]?.InnerText;
                                    hospitalInfo.AsmGrd08 = xmlNode["asmGrd08"]?.InnerText;
                                    hospitalInfo.AsmGrd09 = xmlNode["asmGrd09"]?.InnerText;
                                    hospitalInfo.AsmGrd10 = xmlNode["asmGrd10"]?.InnerText;
                                    hospitalInfo.AsmGrd11 = xmlNode["asmGrd11"]?.InnerText;
                                    hospitalInfo.AsmGrd12 = xmlNode["asmGrd12"]?.InnerText;
                                    hospitalInfo.AsmGrd13 = xmlNode["asmGrd13"]?.InnerText;
                                    hospitalInfo.AsmGrd14 = xmlNode["asmGrd14"]?.InnerText;
                                    hospitalInfo.AsmGrd15 = xmlNode["asmGrd15"]?.InnerText;
                                    hospitalInfo.AsmGrd16 = xmlNode["asmGrd16"]?.InnerText;
                                    hospitalInfo.AsmGrd17 = xmlNode["asmGrd17"]?.InnerText;
                                    hospitalInfo.AsmGrd18 = xmlNode["asmGrd18"]?.InnerText;
                                    hospitalInfo.AsmGrd19 = xmlNode["asmGrd19"]?.InnerText;
                                    hospitalInfo.AsmGrd20 = xmlNode["asmGrd20"]?.InnerText;
                                    hospitalInfo.AsmGrd21 = xmlNode["asmGrd21"]?.InnerText;
                                    hospitalInfo.AsmGrd22 = xmlNode["asmGrd22"]?.InnerText;
                                    hospitalInfo.AsmGrd23 = xmlNode["asmGrd23"]?.InnerText;

                                    hospitalInfoQueue.Enqueue(hospitalInfo);
                                }
                            }

                            bool lastresult = PAGERESULTCONUT > Convert.ToInt32(totalCount);
                            System.Diagnostics.Debug.WriteLine($"lastresult : {lastresult}, PAGERESULTCONUT : {PAGERESULTCONUT}, totalCount : {totalCount}");
                            if (lastresult)
                                break;
                        }
                        else
                        {
                            /*
                             �����ڵ�	�����޽���	                                        ����
                                   1	APPLICATION_ERROR	                                ���ø����̼� ����
                                   10	INVALID_REQUEST_PARAMETER_ERROR	                    �߸��� ��û �Ķ���� ����
                                   12	NO_OPENAPI_SERVICE_ERROR	                        �ش� ����API���񽺰� ���ų� ����
                                   20	SERVICE_ACCESS_DENIED_ERROR	                        ���� ���ٰź�
                                   22	LIMITED_NUMBER_OF_SERVICE_REQUESTS_EXCEEDS_ERROR    ���� ��û����Ƚ�� �ʰ�����
                                   30	SERVICE_KEY_IS_NOT_REGISTERED_ERROR	                ��ϵ��� ���� ����Ű
                                   31	DEADLINE_HAS_EXPIRED_ERROR	                        Ȱ��Ⱓ ����
                                   32	UNREGISTERED_IP_ERROR	                            ��ϵ��� ���� IP
                                   99	UNKNOWN_ERROR	                                    ��Ÿ����
                             */
                            string errorMessage;
                            switch (resultCode)
                            {
                                case "1":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, ���ø����̼� ����";
                                    break;
                                case "10":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, �߸��� ��û �Ķ���� ����";
                                    break;
                                case "12":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, �ش� ����API���񽺰� ���ų� ����";
                                    break;
                                case "20":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, ���� ���ٰź�";
                                    break;
                                case "22":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, ���� ��û����Ƚ�� �ʰ�����";
                                    break;
                                case "30":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, ��ϵ��� ���� ����Ű";
                                    break;
                                case "31":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, Ȱ��Ⱓ ����";
                                    break;
                                case "32":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, ��ϵ��� ���� IP";
                                    break;
                                case "99":
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : {resultMsg}, ��Ÿ����";
                                    break;
                                default:
                                    errorMessage = $"���� �ڵ� : {resultCode}{Environment.NewLine}���� �޼��� : ���ǵ��� ���� �ڵ��Դϴ�.";
                                    break;
                            }
                            cancellationTokenSource.Cancel();
                            MessageBox.Show(errorMessage, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException)
            {

            }
        }

        private async void AddListViewHospitalList(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    while (!hospitalInfoQueue.IsEmpty)
                    {
                        if (hospitalInfoQueue.TryDequeue(out HospitalInfo? hospitalInfo))
                        {
                            if (hospitalInfo is not null)
                            {
                                lv_HospitalList.Invoke(() =>
                                {
                                    ListViewItem listViewItem = new(hospitalInfo.YadmNm);
                                    listViewItem.SubItems.Add(hospitalInfo.ClCdNm);
                                    listViewItem.SubItems.Add(hospitalInfo.Addr);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd01);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd02);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd03);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd04);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd05);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd06);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd07);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd08);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd09);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd10);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd11);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd12);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd13);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd14);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd15);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd16);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd17);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd18);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd19);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd20);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd21);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd22);
                                    listViewItem.SubItems.Add(hospitalInfo.AsmGrd23);
                                    listViewItem.Tag = hospitalInfo;
                                    lv_HospitalList.Items.Add(listViewItem);
                                    lv_HospitalList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                                    lv_HospitalList.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                                });
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {

                
            }
        }
    }

    public record HospitalInfo
    {
        public string Ykiho { get; set; } // ��ȣȭ�� ����ȣ
        public string YadmNm { get; set; } // �������
        public string ClCd { get; set; } // �����ڵ�
        public string ClCdNm { get; set; } // �����ڵ��
        public string Addr { get; set; } // �ּ�
        public string? AsmGrd01 { get; set; } // ���׸� �򰡵��1, �޼�������� �����򰡵�� (1~5���, �������
        public string? AsmGrd02 { get; set; } // ���׸� �򰡵��2, ������ �����򰡵�� (��ȣ�� ��� ����)
        public string? AsmGrd03 { get; set; } // ���׸� �򰡵��3, �������� �����򰡵�� (1~5���, �������
        public string? AsmGrd04 { get; set; } // ���׸� �򰡵��4, �Ƿ�޿����Ű� �����򰡵�� (1~5���, �������
        public string? AsmGrd05 { get; set; } // ���׸� �򰡵��5, �������� �������� �׻��� �����򰡵�� (1~5���, �������
        public string? AsmGrd06 { get; set; } // ���׸� �򰡵��6, ���󵿸ƿ�ȸ�� �����򰡵�� (1~5���, �������
        public string? AsmGrd07 { get; set; } // ���׸� �򰡵��7, �޼���⵵���� �׻��� ó��� �����򰡵�� (1~5���, �������
        public string? AsmGrd08 { get; set; } // ���׸� �򰡵��8, �ֻ��� ó��� �����򰡵�� (1~5���, �������
        public string? AsmGrd09 { get; set; } // ���׸� �򰡵��9, ��ǰ��� �����򰡵�� (1~5���, �������
        public string? AsmGrd10 { get; set; } // ���׸� �򰡵��10, ��纴�� �����򰡵�� (1~5���, �������
        public string? AsmGrd11 { get; set; } // ���׸� �򰡵��11, �索�� �����򰡵�� (��ȣ�� ��� ����)
        public string? AsmGrd12 { get; set; } // ���׸� �򰡵��12, ����� �����򰡵�� (1~5���, �������
        public string? AsmGrd13 { get; set; } // ���׸� �򰡵��13, ���� �����򰡵�� (1~5���, �������
        public string? AsmGrd14 { get; set; } // ���׸� �򰡵��14, ����� �����򰡵�� (1~5���, �������
        public string? AsmGrd15 { get; set; } // ���׸� �򰡵��15, ��� �����򰡵�� (1~5���, �������
        public string? AsmGrd16 { get; set; } // ���׸� �򰡵��16, õ�� �����򰡵�� (��ȣ�� ��� ����)
        public string? AsmGrd17 { get; set; } // ���׸� �򰡵��17, ������⼺����ȯ �����򰡵�� (1~5���, �������
        public string? AsmGrd18 { get; set; } // ���׸� �򰡵��18, ��� �����򰡵�� (1~5���, �������
        public string? AsmGrd19 { get; set; } // ���׸� �򰡵��19, ��ȯ�ڽ� �����򰡵�� (1~5���, �������
        public string? AsmGrd20 { get; set; } // ���׸� �򰡵��20, �Ż�����ȯ�ڽ� �����򰡵�� (1~5���, �������
        public string? AsmGrd21 { get; set; } // ���׸� �򰡵��21, ���� �����򰡵�� (1~5���, �������
        public string? AsmGrd22 { get; set; } // ���׸� �򰡵��22, ���Űǰ� �Կ����� �����򰡵�� (1~5���, �������
        public string? AsmGrd23 { get; set; } // ���׸� �򰡵��23, �޼��ϱ⵵���� �׻��� ó��� �����򰡵�� (1~5���, �������

    }
}