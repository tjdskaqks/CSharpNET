using System.Xml;
using System.Collections.Concurrent;
using System.Web;

namespace hospAsmInfoService_v1
{
    public partial class Form1 : Form
    {
        private const string GET_URL = "http://apis.data.go.kr/B551182/hospAsmInfoService/getHospAsmInfo?serviceKey={0}&pageNo={1}&numOfRows={2}&ykiho=JDQ4MTg4MSM1MSMkMSMkMCMkODkkMzgxMzUxIzExIyQxIyQzIyQ4OSQyNjE4MzIjNTEjJDEjJDYjJDgz";
        private const string SERVICEKEY = "개인키";
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
            lv_HospitalList.Columns.Add("병원", 100, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("종별코드명", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("주소", 150, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("급성기뇌졸중", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("고혈압", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("혈액투석", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("의료급여정신과", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("수술부위 감염예방 항생제", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("관상동맥우회술", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("급성상기도감염 항생제 처방률", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("주사제 처방률", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("약품목수", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("요양병원", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("당뇨병", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("대장암", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("위암", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("유방암", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("폐암", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("천식", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("만성폐쇄성폐질환", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("폐렴", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("중환자실", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("신생아중환자실", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("마취", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("정신건강 입원영역", 50, HorizontalAlignment.Center);
            lv_HospitalList.Columns.Add("급성하기도감염 항생제 처방률", 50, HorizontalAlignment.Center);
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
                         정상코드	정상메시지	    설명
                               00   NORMAL SERVICE. 성공
                         */
                        string resultCode = xmlNodeHeaderList[0]["resultCode"].InnerText;
                        string resultMsg = xmlNodeHeaderList[0]["resultMsg"].InnerText;

                        if (resultCode.Equals("00"))
                        {
                            using XmlNodeList xmlNodeBodyrList = xmlDocument1.GetElementsByTagName("body");

                            pageNo = Convert.ToInt32(xmlNodeBodyrList[0]["pageNo"].InnerText);
                            pageNo++;
                            string totalCount = xmlNodeBodyrList[0]["totalCount"].InnerText;
                            this.Invoke(() => { this.Text = $"totalCount : {totalCount} 조회 성공"; });
                            
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
                             에러코드	에러메시지	                                        설명
                                   1	APPLICATION_ERROR	                                어플리케이션 에러
                                   10	INVALID_REQUEST_PARAMETER_ERROR	                    잘못된 요청 파라메터 에러
                                   12	NO_OPENAPI_SERVICE_ERROR	                        해당 오픈API서비스가 없거나 폐기됨
                                   20	SERVICE_ACCESS_DENIED_ERROR	                        서비스 접근거부
                                   22	LIMITED_NUMBER_OF_SERVICE_REQUESTS_EXCEEDS_ERROR    서비스 요청제한횟수 초과에러
                                   30	SERVICE_KEY_IS_NOT_REGISTERED_ERROR	                등록되지 않은 서비스키
                                   31	DEADLINE_HAS_EXPIRED_ERROR	                        활용기간 만료
                                   32	UNREGISTERED_IP_ERROR	                            등록되지 않은 IP
                                   99	UNKNOWN_ERROR	                                    기타에러
                             */
                            string errorMessage;
                            switch (resultCode)
                            {
                                case "1":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 어플리케이션 에러";
                                    break;
                                case "10":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 잘못된 요청 파라메터 에러";
                                    break;
                                case "12":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 해당 오픈API서비스가 없거나 폐기됨";
                                    break;
                                case "20":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 서비스 접근거부";
                                    break;
                                case "22":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 서비스 요청제한횟수 초과에러";
                                    break;
                                case "30":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 등록되지 않은 서비스키";
                                    break;
                                case "31":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 활용기간 만료";
                                    break;
                                case "32":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 등록되지 않은 IP";
                                    break;
                                case "99":
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : {resultMsg}, 기타에러";
                                    break;
                                default:
                                    errorMessage = $"에러 코드 : {resultCode}{Environment.NewLine}에러 메세지 : 정의되지 않은 코드입니다.";
                                    break;
                            }
                            cancellationTokenSource.Cancel();
                            MessageBox.Show(errorMessage, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        public string Ykiho { get; set; } // 암호화된 요양기호
        public string YadmNm { get; set; } // 요양기관명
        public string ClCd { get; set; } // 종별코드
        public string ClCdNm { get; set; } // 종별코드명
        public string Addr { get; set; } // 주소
        public string? AsmGrd01 { get; set; } // 평가항목 평가등급1, 급성기뇌졸중 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd02 { get; set; } // 평가항목 평가등급2, 고혈압 병원평가등급 (양호한 기관 공개)
        public string? AsmGrd03 { get; set; } // 평가항목 평가등급3, 혈액투석 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd04 { get; set; } // 평가항목 평가등급4, 의료급여정신과 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd05 { get; set; } // 평가항목 평가등급5, 수술부위 감염예방 항생제 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd06 { get; set; } // 평가항목 평가등급6, 관상동맥우회술 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd07 { get; set; } // 평가항목 평가등급7, 급성상기도감염 항생제 처방률 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd08 { get; set; } // 평가항목 평가등급8, 주사제 처방률 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd09 { get; set; } // 평가항목 평가등급9, 약품목수 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd10 { get; set; } // 평가항목 평가등급10, 요양병원 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd11 { get; set; } // 평가항목 평가등급11, 당뇨병 병원평가등급 (양호한 기관 공개)
        public string? AsmGrd12 { get; set; } // 평가항목 평가등급12, 대장암 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd13 { get; set; } // 평가항목 평가등급13, 위암 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd14 { get; set; } // 평가항목 평가등급14, 유방암 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd15 { get; set; } // 평가항목 평가등급15, 폐암 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd16 { get; set; } // 평가항목 평가등급16, 천식 병원평가등급 (양호한 기관 공개)
        public string? AsmGrd17 { get; set; } // 평가항목 평가등급17, 만성폐쇄성폐질환 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd18 { get; set; } // 평가항목 평가등급18, 폐렴 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd19 { get; set; } // 평가항목 평가등급19, 중환자실 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd20 { get; set; } // 평가항목 평가등급20, 신생아중환자실 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd21 { get; set; } // 평가항목 평가등급21, 마취 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd22 { get; set; } // 평가항목 평가등급22, 정신건강 입원영역 병원평가등급 (1~5등급, 등급제외
        public string? AsmGrd23 { get; set; } // 평가항목 평가등급23, 급성하기도감염 항생제 처방률 병원평가등급 (1~5등급, 등급제외

    }
}