

| Scene ID | S01\_THE\_LEAKED\_IMAGE |
| :---- | :---- |
| **Bối cảnh** | Phòng truyền thông của Lễ hội Sắc Việt Trẻ |
| **Nhân vật** | Người chơi, Vy, Hương |
| **Cách chơi** | Chỉ đọc hội thoại và chọn 1 trong 3 đáp án |
| **Scene tiếp theo** | S02\_THE\_SECRET\_DOCUMENT |

# **1\. Logic tổng thể của Scene 1**

**Leak xuất hiện  → Choice 1 → MERGE → Vy xác nhận ảnh thật → Choice 2 → MERGE → bằng chứng về quy trình tuyển chọn được tìm thấy → Choice 3 → hậu quả → Learning Card → Scene 2** 

# Lưu ý cho dev: tất cả lựa chọn đều dùng cùng màu. Mỗi đáp án chỉ tạo phản hồi ngắn và cập nhật điểm, sau đó quay lại node chung tiếp theo. 

# **Focus Character**:

Vy: trưởng nhóm tình nguyện viên 

+ Tính cách: có năng lực, độc lập, k muốn bị xem như 1 nạn nhân bất lực, đang chịu áp lực vì hình ảnh bị phát tán.  
+ vai trò: đại diện quyền riêng tư, cyberbullying, consent, có trách no khi xử lí nội dung nhạy cảm

cô Hương: Trưởng ban tổ chức lễ hội

+ tính cách: muốn sự kiện thành công, quan tâm đến hình ảnh tổ chức, đôi lúc trì hoãn công khai vấn đề, không hoàn toàn trong sạch nhưng cũng không phải phản diện.  
+ vai trò: institutional transparency, khủng hoảng truyền thông, xung đột giữa danh tiếng và quyền được biết.

MIL CORE:  
Privacy, consent, public interest và responsible disclosure

CORE QUESTION:  
“Something is true/private and leaked. Does public interest give us the right to share it?” 

## **Stake 1 — Vy**

Nếu xử lý sai:

* hình ảnh tiếp tục lan;  
* đời tư của Vy bị soi mói;  
* người khác tìm tài khoản cá nhân;  
* gia đình hoặc bạn bè có thể bị kéo vào;  
* Vy bị gắn với allegation “đi cửa sau”;  
* uy tín của Vy trong vai trò trưởng volunteer giảm;  
* dù allegation sau này được bác bỏ, ảnh và screenshot vẫn tồn tại.

Quan trọng hơn:

> Vy mất quyền kiểm soát câu chuyện về chính mình.

Đây không chỉ là “Vy buồn”.

Đây là **privacy harm \+ reputational harm \+ loss of agency**.

---

## **Stake 2 — Tổ chức / cô Hương**

Nếu xử lý quá chậm hoặc quá kín:

* cộng đồng nghĩ BTC che giấu favoritism;  
* niềm tin vào quy trình tuyển volunteer giảm;  
* media có thể tiếp tục đào sâu;  
* sponsor có thể hỏi về integrity của lễ hội;  
* crisis từ Scene 0 chưa dứt lại xuất hiện thêm scandal mới.

Nếu xử lý quá mạnh:

* BTC có thể vô tình xác nhận rằng ảnh “quan trọng”;  
* repost ảnh khiến nội dung lan rộng hơn;  
* cố bảo vệ danh tiếng có thể biến BTC thành bên xâm phạm quyền riêng tư của Vy.

Tức là cô Hương có **lý do hợp lý** để gây pressure.

# **1\. Full Script — Scene 1**

## **S1.0.0 — Mở đầu**

Chưa đầy hai giờ trước khi lễ hội mở cửa.

Sau vụ việc của Mai An, phòng truyền thông tiếp tục nhận thêm hàng loạt thông báo.

**CÔ HƯƠNG — căng thẳng**

Có một chuyện khác đang lan rất nhanh.

Dang co một bức ảnh chụp **Vy**, trưởng nhóm tình nguyện viên, đang ngồi riêng tại một quán café với **anh Quân**, một thành viên trong ban tổ chức.

Caption của bài đăng viết:

> “Không khó hiểu vì sao Vy được chọn làm trưởng nhóm tình nguyện viên.”

Một số bình luận đã bắt đầu gọi đây là:

> “Quan hệ nội bộ.”

> “Đi cửa sau.”

> “Tuyển người không minh bạch.”

Bài đăng đã có hơn 2.000 lượt chia sẻ.

**CÔ HƯƠNG**

Ban tai tro vừa gửi cho cô screenshot này.

Họ muốn biết ro su tinh nay trong ban tổ chức va xac thuc vu viec co dung hay không.

Nếu mình im lặng, chuyện này sẽ trông như đang che giấu.

**VY — bình tĩnh nhưng khó chịu**

Ảnh đó là ảnh của em.

Nhưng em không biết ai chụp, và em chưa từng đồng ý cho nó được đăng.

**CÔ HƯƠNG**

Vậy ít nhất mình biết ảnh là thật.

Cô nghĩ chúng ta nên phản hồi ngay trước khi câu chuyện đi xa hơn.

---

## **S1.0.1Q — Đội truyền thông nên làm gì trước?**

**Người chơi chọn một trong ba đáp án:**

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Mình nên đăng lại ảnh kèm thông báo rằng BTC đang xác minh. Như vậy mọi người biết chính xác mình đang nói về chuyện gì.”**  | Privacy −2 Transparency \+1 Crisis \+2 Vy Trust −2  | VY: Em hiểu cần phản hồi, nhưng đăng lại có nghĩa page chính thức đang đưa bức ảnh đến thêm hàng chục nghìn người. Hậu quả: Bài của BTC được chia sẻ rộng. Một số người lần đầu biết đến bức ảnh thông qua chính thông báo “đang xác minh”. → S01\_MERGE\_01 |
| **B** | **“Không cần đăng lại ảnh. Mình có thể xác nhận rằng BTC đang kiểm tra cáo buộc về quy trình tuyển chọn.”** | Privacy \+2 Transparency \+1 Vy Trust \+1 Crisis −1  | **CÔ HƯƠNG:** “Được. Nhưng thông báo phải đủ rõ để công chúng không nghĩ mình đang né tránh.”   **Hậu quả:** BTC phản hồi mà không khuếch đại thêm bức ảnh.  → `S01_MERGE_01`  |
| **C** | **“Đây là ảnh riêng tư của Vy. Mình không nên phản hồi gì cả.”**  | Privacy \+1 Transparency −2 Crisis \+1  | CÔ HƯƠNG: “Nếu người ta chỉ bàn chuyện đời tư thì cô đồng ý. Nhưng họ đang dùng ảnh để đặt câu hỏi về quy trình của lễ hội.”**Hậu quả**: Trong khoảng trống thông tin, các bài đăng khác bắt đầu thêm suy đoán về cách Vy được chọn.  → `S01_MERGE_01`  |

---

# **S1.0.2 — MERGE 01: Ảnh thật có chứng minh cáo buộc?**

**CÔ HƯƠNG**

Cô cần biết chính xác hôm đó Vy có gặp Quân không?

**VY**

Có. Em gặp anh Quân tại quán café đó.

**CÔ HƯƠNG**

Vậy ít nhất phần đó đúng.

Nếu công chúng hỏi, chúng ta phải giải thích mối quan hệ giữa hai người.

**VY — cứng rắn**

Khoan đã. Em xác nhận bức ảnh là thật.

Em chưa xác nhận caption của họ là thật.

**VY**

Một bức ảnh cho thấy em gặp anh Quân.

Nó không tự chứng minh rằng em được chọn vì anh ấy.

## **S1.0.3Q — Bức ảnh cho phép chúng ta kết luận điều gì?**

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Bức ảnh cho thấy Vy và một thành viên ban tổ chức có sự quen biết từ trước. Điều này đặt ra câu hỏi về tính độc lập của quyết định tuyển chọn.”**  | Evidence −2 Privacy −1 Vy Trust −1 Crisis \+1  | VY: “Đã gặp nhau” và “được ưu ái” là hai kết luận khác nhau. Nếu muốn biết em được chọn thế nào, mình nên kiểm tra quy trình tuyển chọn. Hậu quả: Trong draft statement, bức ảnh bắt đầu được dùng như bằng chứng về khả năng có conflict of interest dù chưa xác minh. → S01\_MERGE\_02  |
| **B** | **“Ảnh chỉ xác nhận Vy và Quân đã gặp nhau. Lý do gặp và việc nó có liên quan đến tuyển chọn hay không thì chưa biết.”** | Evidence \+2 Transparency \+1 Vy Trust \+1  | Vy: Đúng. Nếu vấn đề là em được chọn như thế nào thì hãy kiểm tra bằng chứng về việc đó. **Hậu quả:** Đội truyền thông tách được **fact** khỏi **inference**. → S01\_MERGE\_02 |
| **C** | **“Vì Vy không đồng ý cho đăng ảnh nên đăng ảnh sẽ xâm phạm riêng tư, chúng ta không nên xem nó như bất kỳ loại bằng chứng nào.”** | Privacy \+1 Evidence −1 Transparency −1  | CÔ HƯƠNG: Quyền riêng tư là một vấn đề. Nhưng nếu ảnh đặt ra một câu hỏi thật về quy trình của chúng ta, mình vẫn phải kiểm tra câu hỏi đó bằng những nguồn phù hợp hơn. Hậu quả: BTC bảo vệ quyền riêng tư nhưng chưa giải quyết được nghi vấn về tính minh bạch. → S01\_MERGE\_02  |

---

# **S1.0.4 — MERGE 02: Kiểm tra allegation thay vì đời tư**

Người chơi và cô Hương tìm thấy hồ sơ tuyển chọn nội bộ của lễ hội.

Thông tin cho thấy:

* Vy nộp đơn làm trưởng nhóm tình nguyện viên trước buổi gặp trong ảnh.  
* Vy được phỏng vấn bởi một panel ba người.  
* Quyết định chọn Vy được xác nhận **hai ngày trước** thời điểm bức ảnh được chụp.  
* Anh Quân không nằm trong hội chấm ứng viên.  
* Buổi gặp tại café diễn ra sau khi Vy đã được chọn.

**CÔ HƯƠNG — nhẹ nhõm**

Vậy là rõ rồi.

Mình có thể đăng timeline, biên bản tuyển chọn và cả bức ảnh để chứng minh cuộc gặp diễn ra sau quyết định.

**VY**

Tại sao vẫn phải đăng ảnh?

Nếu timeline và quy trình đã trả lời được allegation thì công chúng cần ảnh của em để làm gì?

Cô Hương dừng lại.

**CÔ HƯƠNG**

Cô muốn câu trả lời đủ thuyết phục.

Nếu thiếu bằng chứng trực quan, người ta có thể nói chúng ta chỉ đang tự bảo vệ mình.

**VY**

Minh bạch về **quy trình** không có nghĩa phải công khai thêm **đời tư của em**.

---

# **CHOICE 3**

## **S1.0.5Q — BTC nên phản hồi công khai như thế nào?**

## 

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Nên công bố tất cả tài liệu ảnh, timeline và các tin nhắn liên quan. Càng nhiều bằng chứng càng minh bạch.”**  | Transparency \+2 Privacy −3 Empathy −1 Vy Trust −2 Crisis \+1  | CÔ HƯƠNG: Cách này sẽ cho thấy chúng ta không giấu gì. VY: Nhưng nó cũng biến tất cả thông tin cá nhân của em thành tài liệu để công chúng phân tích. Hậu quả: Bài đính chính bác bỏ được tranh cãi về tuyển chọn. Nhưng: ảnh của Vy tiếp tục được repost; screenshot tin nhắn riêng bị tách khỏi bài giải thích; người dùng bắt đầu soi những mối quan hệ khác của Vy; một số tài khoản tìm và tag người thân của cô. BTC đã chứng minh được sự minh bạch, nhưng đồng thời xâm phạm quyền riêng tư của Vy. → S01\_END |
| **B** | **“Công bố timeline tuyển chọn, thành viên tham gia chấm điểm và thời điểm quyết định. Nói rõ những dữ liệu cá nhân nào không được công khai và vì sao. ”** | Evidence \+2 Transparency \+2 Privacy \+2 Empathy \+1 Vy Trust \+2 Crisis −2  | CÔ HƯƠNG:“Nếu mình công bố cả tiêu chí và cách ra quyết định, mọi người sẽ bắt đầu soi từng bước của quy trình tuyển chọn. Chỉ cần có một điểm chưa rõ, câu chuyện có thể chuyển sang cách ban tổ chức vận hành.” Hậu quả: Ban tổ chức phải cân nhắc thông tin nào đủ để kiểm chứng cáo buộc mà không mở toàn bộ quy trình nội bộ. Phản hồi giúp giảm suy đoán về Vy, nhưng đồng thời khiến một số người bắt đầu đặt câu hỏi về tính minh bạch của quy trình tuyển chọn. → S01\_END |
| **C** | **“BTC nên khẳng định mọi cáo buộc đang lan truyền hoàn toàn sai và yêu cầu cộng đồng ngừng chia sẻ.”** | Transparency −2 Evidence −1 Crisis \+2 Vy Trust \+1  | VY: “Nhưng ảnh là thật. Nếu mình gọi tất cả là giả, họ chỉ cần repost ảnh để nói BTC đang nói dối.” Hậu quả: Người dùng repost ảnh để chứng minh statement của BTC “không trung thực”. Cuộc tranh luận chuyển từ allegation ban đầu sang: “Ban tổ chức đang che giấu điều gì?” Crisis tiếp tục tăng. → S01\_END |

# **S1.0.6 — Kết thúc Scene 1**

## **CASE FILE 02 — TRUE, BUT NOT YOURS TO SHARE**

**Bài học**

Một hình ảnh có thể hoàn toàn thật nhưng điều đó không tự động tạo quyền để công khai hoặc tái sử dụng nó.

Khi xử lý thông tin cá nhân, cần tách ba câu hỏi:

> **Nó có thật không?**

> **Nó có thực sự chứng minh điều đang được tuyên bố không?**

> **Có cần công khai nó để giải quyết vấn đề không?**

Minh bạch không có nghĩa là công bố mọi thông tin đang có.

Một phản hồi có trách nhiệm nên sử dụng **lượng thông tin tối thiểu cần thiết** để trả lời vấn đề công chúng có quyền quan tâm, đồng thời hạn chế tổn hại không cần thiết cho người liên quan.

### **Câu hỏi cần nhớ**

> **Can I address the issue without exposing the person?**

