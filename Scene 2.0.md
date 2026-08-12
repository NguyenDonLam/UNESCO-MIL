## **Scene 2 Multiple-Choice Script — Draft**

| Scene ID | S02\_THE\_SECRET\_DOCUMENT |
| :---- | :---- |
| **Bối cảnh** | Phòng truyền thông của Lễ hội Sắc Việt Trẻ |
| **Nhân vật** | Người chơi, Cô Hương, Minh |
| **Cách chơi** | Chỉ đọc hội thoại và chọn 1 trong 3 đáp án |
| **Scene tiếp theo** | S03\_PUBLIC\_EMERGENCY |

# **1\. Logic tổng thể của Scene 2**

**Mở đầu ngắn → Choice 0.1 → phản hồi riêng → MERGE → Choice 0.2 → phản hồi riêng → MERGE → sự thật được tiết lộ → Choice 0.3 → hậu quả riêng → MERGE → Scene 1**

Lưu ý cho dev: tất cả lựa chọn đều dùng cùng màu. Mỗi đáp án chỉ tạo phản hồi ngắn và cập nhật điểm, sau đó quay lại node chung tiếp theo. 

Focus Character:

Minh: Thành viên kĩ thuật trong nhóm truyền thông

+ Tính cách: giỏi công nghệ, thích dữ liệu, ít quan tâm đến cách giao tiếp, đôi khi quá tin vào AI detector và công cụ kỹ thuật  
+ Vai trò: technical verification, AI detection

cô Hương: Trưởng ban tổ chức lễ hội

+ tính cách: muốn sự kiện thành công, quan tâm đến hình ảnh tổ chức, đôi lúc trì hoãn công khai vấn đề, không hoàn toàn trong sạch nhưng cũng không phải phản diện.  
+ vai trò: institutional transparency, khủng hoảng truyền thông, xung đột giữa danh tiếng và quyền được biết.

### **MIL trọng tâm**

**Information security \+ evidence preservation \+ financial/data literacy \+ communication under uncertainty** 

### **Câu hỏi trung tâm**

> **Khi một tài liệu nội bộ bị leak và tạo ra cáo buộc nghiêm trọng, làm sao vừa hạn chế thiệt hại, vừa kiểm tra đúng vấn đề, vừa không nói nhiều hơn những gì mình biết?** 

### **Logic tổng thể**

**Tài liệu nội bộ xuất hiện → Choice 1 → MERGE → phát hiện mối quan hệ với vendor → Choice 2 → MERGE → kiểm tra procurement → Choice 3 → hậu quả → Learning Card → Scene 3**

---

# **1\. Full Script — Scene 2**

## **S2.0.0 — Mở đầu**

Chỉ còn hơn một giờ trước khi lễ hội bắt đầu. Và sau hai cuộc khủng hoảng liên tiếp, ban tổ chức đang trong một bầu không khí căng thẳng,

**CÔ HƯƠNG — căng thẳng**

Có một tài liệu nội bộ đang bị phát tán. Một file ngân sách nội bộ của lễ hội đang bị chia sẻ trên mạng. 

Một dòng được highlight:

> **External Production & Advisory Services — 480,000,000 VND**

Bên cạnh là tên vendor:

> **Minh Phát Media**

Một bài khác viết:

> “Gần nửa tỷ tiền tài trợ đã bị chuyển vào một hạng mục mập mờ. Lễ hội văn hóa hay dự án rút ruột tài trợ?”

**MINH**

Tài liệu này khớp với mẫu ngân sách nội bộ của mình.

Tên vendor và khoản 480 triệu cũng có trong hệ thống.

**CÔ HƯƠNG**

Sponsor vừa gửi email hỏi cô chuyện này.

Nếu họ nghĩ tiền bị sử dụng sai, họ có thể yêu cầu giải trình ngay trong hôm nay.

**MINH**

Có chuyện nghiêm trọng hơn. Link gốc dẫn tới một folder dùng chung của ban tổ chức.

Folder này từng được bật chế độ:

> **Anyone with the link can view.**

Và link vẫn đang hoạt động.

File chứa hợp đồng vendor, bảng ngân sách và một số thông tin liên hệ nội bộ.

Em muốn khóa toàn bộ quyền truy cập ngay.

---

# **CHOICE 1**

# **S2.0.1Q — Nên xử lý hệ thống trước như thế nào?** 

**Người chơi chọn một trong ba đáp án:**

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Khóa toàn bộ folder và tài khoản liên quan ngay. Ngăn việc rò rỉ ngay lập tức.”** | Privacy \+2 Crisis −1 Evidence −1 Transparency −1 Minh Trust \+2  | CÔ HƯƠNG: “Nhưng team hậu cần đang không mở được hợp đồng và danh sách vendor.”  Hậu quả: leak từ folder dừng nhanh, nhưng hoạt động festival bị gián đoạn; một số payment và check-in vendor bị chậm. Việc khóa quá rộng cũng khiến team khó truy cập được tài liệu cần thiết. → S02\_MERGE\_01 |
| **B** | **“Chặn link đang công khai nhưng giữ quyền cho các tài khoản nội bộ đang cần làm việc. Đồng thời rà lại xem còn link nào khác đang mở.”** | Privacy \+1 Evidence \+2 Minh Trust \+1  | **MINH:** “Em cần thời gian rà từng quyền truy cập. Chặn một link chưa chắc có nghĩa mọi đường vào đều đã đóng.”  **CÔ HƯƠNG:** “Miễn là vận hành vẫn tiếp tục được.”   **Hậu quả:** đường truy cập đang được chia sẻ bị chặn nhưng Minh chưa thể khẳng định leak đã được contain hoàn toàn. Một số nhân viên phải xác minh lại quyền.  → `S02_MERGE_01`  |
| **C** | **“Giữ folder hoạt động thêm một lúc. Minh theo dõi xem tài khoản nào đang truy cập, còn đội truyền thông xử lý sponsor và dư luận trước.”** | Evidence \+2 Privacy −2 Transparency \+1 Crisis \+1  | **MINH:** “Việc giữ nguyên như giờ sẽ giúp mình nhìn rõ hơn activity đang xảy ra, nhưng em không thể đảm bảo không có thêm file bị lấy.”  **CÔ HƯƠNG:** “Cô thấy ưu tiên it nhất team vận hành không bị gián đoạn và mình có thời gian trả lời sponsor.”   **Hậu quả:** team tiếp tục làm việc bình thường, nhưng trước khi quyền được thay đổi, một file vendor khác bị tải xuống.  → `S02_MERGE_01`  |

## 

---

# **S2.0.2 — MERGE 01:** 

**CÔ HƯƠNG**

Giờ quay lại khoản 480 triệu. Với hạng mục “External Production & Support.”, khoản đó gồm sân khấu, livestream, nhân sự kỹ thuật và một số dịch vụ bên ngoài.

**MINH**

Em mới chỉ đối chiếu được bảng ngân sách tổng.

Em chưa kiểm tra:

* 480 triệu là ngân sách dự kiến hay số tiền đã chi;  
* đã thanh toán bao nhiêu;  
* các hợp đồng có khớp không.

Mình vẫn chưa biết toàn bộ số tiền đó đã được sử dụng như thế nào.

**CÔ HƯƠNG**

Nhà tài trợ đang chờ câu trả lời.

Nếu mình nói “chưa biết”, họ sẽ nghĩ ban tổ chức không kiểm soát nổi ngân sách.

---

# **CHOICE 2**

## **S2.0.3Q — BTC nên phản hồi thế nào?**

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Nói rõ 480 triệu là ngân sách cho nhiều dịch vụ, không phải một khoản tiền thiếu minh bạch’. Mình cần phản bác claim đang viral trước.”** | Transparency \+1 Evidence −1 Crisis −1  | **CÔ HƯƠNG:** “Cách này ít nhất ngăn câu chuyện đi xa hơn.”  **MINH:** “Nhưng mình vẫn chưa kiểm tra việc chi tiêu thực tế. Nếu gọi claim là sai hoàn toàn, statement sẽ mạnh hơn bằng chứng hiện tại.”   **Hậu quả:** bài đăng “480 triệu biến mất” mất đà trong ngắn hạn. Tuy nhiên BTC giờ phải bảo vệ một statement khá chắc chắn khi investigation vẫn đang diễn ra.  → `S02_MERGE_02`  |
| **B** | **“Xác nhận 480 triệu là ngân sách cho nhiều hạng mục sản xuất và hỗ trợ. Nói rõ BTC vẫn đang đối chiếu số đã chi và sẽ cập nhật khi kiểm tra xong.”**  | Evidence \+2 Transparency \+2  | **CÔ HƯƠNG:** “Nhà tài trợ sẽ yêu cầu giải thích chi tiết hơn.”  **MINH:** “Nhưng mình đang xác nhận rõ phần nào đã biết và phần nào chưa.”   **Hậu quả:** không có bài đăng mới quá nghiêm trọng, nhưng một số bình luận cho rằng BTC đang ‘câu giờ’. Pressure chưa biến mất.  → `S02_MERGE_02`  |
| **C** | **“Chưa nói gì về con số. Chỉ xác nhận tài liệu nội bộ đã bị truy cập trái phép và BTC đang xử lý.”** | Evidence \+1 Transparency −2 Privacy \+1 Crisis \+2  | **CÔ HƯƠNG:** “Ít nhất ban tổ chức không phát biểu khi chưa chắc.”  **MINH:** “Nhưng chuyện file bị leak và chuyện 480 triệu được dùng thế nào là hai câu hỏi khác nhau.”   **Hậu quả:** một page đăng headline: **‘BTC chỉ nói về vụ rò rỉ, không trả lời tiền tài trợ đi đâu.’**  → `S02_MERGE_02`  |

# **S2.0.3 — Kiểm tra hồ sơ** 

**MINH**

Em và team tài chính vừa đối chiếu được thêm.

480 triệu không phải số tiền đã thanh toán. Đó là **mức ngân sách tối đa được phê duyệt** cho toàn nhóm External Production & Support. 

Trong 312 triệu đã thanh toán có một khoản:

> **36,000,000 VND — Emergency Technical Support**

Đây là dịch vụ sửa hệ thống sân khấu sau sự cố tuần trước.

Team kỹ thuật xác nhận công việc đã được thực hiện.

Invoice chi tiết chưa được gửi đầy đủ cho bộ phận tài chính.

Họ mới có xác nhận thanh toán và biên bản công việc.

**CÔ HƯƠNG**

Tức là  số tiền 480 triệu được chi tiêu hợp lí, chỉ là paperwork chưa hoàn tất.

Nếu đưa 36 triệu vào statement, mọi người sẽ chỉ nhìn thấy một con số mới để nghi ngờ.

**MINH**

Nếu bỏ nó đi và tài liệu khác leak sau đó, họ sẽ hỏi tại sao BTC biết mà không nói.

**CÔ HƯƠNG**

Vậy câu trả lời nào vừa chính xác vừa không tự tạo thêm scandal?

---

# **CHOICE 3**

## **S2.0.3Q — Sau khi kiểm tra, BTC nên công bố kết quả thế nào?** 

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Công bố rằng 480 triệu là ngân sách tối đa và không có bằng chứng tiền tài trợ bị thất thoát. Không cần đưa khoản 36 triệu vào vì nó không chứng minh misuse.”** | Crisis −2 Transparency −1 Evidence \+1  | **MINH:** “Không sai. Nhưng mình đang bỏ qua một vấn đề chưa được làm rõ.” **Hậu quả:** scandal giảm nhanh. Nhà tài trợ tạm yên tâm. Tuy nhiên, nếu hồ sơ 36 triệu xuất hiện sau đó, BTC sẽ phải giải thích vì sao nó không được nhắc tới trong lần công bố đầu.  → `S02_END_A`  |
| **B** | **“Công bố 480 triệu là ngân sách tối đa, 312 triệu đã được thanh toán và phần lớn có đủ hồ sơ. Đồng thời nói rõ khoản 36 triệu có dịch vụ thực tế nhưng hồ sơ chi tiết vẫn đang được bổ sung.”** | Evidence \+2 Transparency \+2  | **CÔ HƯƠNG:** “Nhà tài trợ sẽ yêu cầu giải thích chi tiết hơn.”  **MINH:** “Nhưng mình đang xác nhận rõ phần nào đã biết và phần nào chưa.”   **Hậu quả:** không có bài đăng mới quá nghiêm trọng, nhưng một số bình luận cho rằng BTC đang ‘câu giờ’. Pressure chưa biến mất.  → `S02_MERGE_02`  |
| **C** | **“Vì vẫn có 36 triệu chưa đủ hồ sơ, BTC nên nói đang điều tra thêm về một phần tiền tài trợ.”**  | Transparency \+1 Evidence −2 Crisis \+2  | **MINH:** “Thiếu invoice nên mình sẽ follow up, nhưng mình không có evidence về misuse.”  **CÔ HƯƠNG:** “Nếu mình dùng cụm đó, mọi page sẽ coi như BTC vừa thừa nhận scandal.”   **Hậu quả:** headline xuất hiện: **‘BTC thừa nhận đang điều tra sử dụng sai tiền tài trợ.’** Sponsor yêu cầu giải trình khẩn cấp và các allegation cũ được chia sẻ lại.  → `S02_END_C`  |

---

# **S2.0.4 — Kết thúc Scene 2**

## **CASE FILE 03 — SUSPICION IS NOT PROOF**

### **Bài học**

> ### **Bài học 4 — Responsible transparency có thể không thoải mái**

**Một statement chính xác có thể:**

* **không dập scandal ngay;**  
* **khiến sponsor hỏi thêm;**  
* **tạo thêm công việc.**

**Nhưng mục tiêu của communication không phải chỉ:**

> **làm controversy biến mất nhanh nhất**

**mà là:**

> **nói đúng mức mà evidence cho phép.**

> ### **Câu hỏi cần nhớ**

> **What do we know, what is still missing, and what can we responsibly claim?**

> 

# **S2.0.5 — Teaser Scene 3**

**MINH**

Có thêm tin nhắn đang được forward.

Một người nói nhiều khách ở khu food court đang bị ngộ độc và khuyên mọi người không tới lễ hội.

**CÔ HƯƠNG**

Có ai xác nhận chưa?

**MINH**

Chưa.

Nhưng nếu đây là nguy cơ sức khỏe thật, mình cũng không thể chờ quá lâu.

**NEXT SCENE:** `S03_PUBLIC_EMERGENCY`

