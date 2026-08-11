 BEFORE YOU SHARE

**Scene 0 Multiple-Choice Script — Revised**  
 *Bản sửa: các lựa chọn trả lời trực tiếp câu hỏi của Linh và không tô sẵn đáp án đúng*

 

| Scene ID | S00\_VIRAL\_POST |
| :---- | :---- |
| **Bối cảnh** | Phòng truyền thông của Lễ hội Sắc Việt Trẻ |
| **Nhân vật** | Người chơi, Linh, Minh |
| **Cách chơi** | Chỉ đọc hội thoại và chọn 1 trong 3 đáp án |
| **Scene tiếp theo** | S01\_THE\_LEAKED\_IMAGE |

# **1\. Logic tổng thể của Scene 0**

**Mở đầu ngắn → Choice 0.1 → phản hồi riêng → MERGE → Choice 0.2 → phản hồi riêng → MERGE → sự thật được tiết lộ → Choice 0.3 → hậu quả riêng → MERGE → Scene 1**

Lưu ý cho dev: tất cả lựa chọn đều dùng cùng màu. Mỗi đáp án chỉ tạo phản hồi ngắn và cập nhật điểm, sau đó quay lại node chung tiếp theo. 

# **2\. Full Script — Scene 0**

## **S0.0.0 — Mở đầu**

**Chỉ còn ba giờ trước khi Lễ hội Sắc Việt Trẻ mở cửa.**

**Bạn là thành viên mới của đội truyền thông.**

**LINH** *— lo lắng*

Có chuyện rồi.

**LINH**

Một đoạn clip về Mai An, ca sĩ đại diện lễ hội, đang lan khắp mạng xã hội.

**LINH**

Trong clip, cô ấy nói: “Người ở An Phúc không hiểu nghệ thuật.”

**LINH**

Clip chỉ dài tám giây, nhưng đã có hàng nghìn lượt chia sẻ. Mọi người đang yêu cầu ban tổ chức hủy tiết mục của cô ấy.

**LINH**

Tớ có một trang gần 50.000 người theo dõi. Tớ nên xử lý đoạn clip này thế nào?

## **S0.0.1Q — Linh nên xử lý đoạn clip thế nào?**

**Người chơi chọn một trong ba đáp án:**

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Cậu cứ đăng lại để cảnh báo mọi người. Clip đang lan nhanh nên không thể chờ lâu.”** | Evidence −2 Linh Trust \+1 Crisis \+2 | LINH: “Được, tớ sẽ đăng ngay. Nếu có vấn đề thật thì mọi người cần biết càng sớm càng tốt.” Hậu quả: đoạn clip tiếp cận thêm hàng chục nghìn người qua trang của Linh. Sau đó → S00\_MERGE\_01 |
| **B** | **“Đừng đăng lại vội. Hãy tìm bài đăng đầu tiên và bản phỏng vấn đầy đủ trước.”** | Evidence \+2 Transparency \+1 Linh Trust \+1 Crisis −1 | LINH: “Được, tớ sẽ chờ. Nhưng mình cần kiểm tra nhanh vì bài đăng đang tiếp tục lan.” Hậu quả: Linh tạm thời không chia sẻ clip. Sau đó → S00\_MERGE\_01 |
| **C** | **“Đừng chia sẻ lại clip. Cậu có thể đăng rằng thông tin đang được xác minh và nhắc mọi người chưa nên kết luận.”** | Evidence \+1 Transparency \+2 Linh Trust \+1 Crisis −1 | LINH: “Ừ, cách này hợp lý hơn. Tớ vẫn có thể cảnh báo mọi người mà không làm đoạn clip lan thêm.” Hậu quả: Linh đăng một thông báo trung lập, nhưng một số người tò mò bắt đầu tìm đoạn clip gốc. Sau đó → S00\_MERGE\_01 |

 

## **S0.0.2 — Cả ba lựa chọn quay lại đây**

**MINH** *— nghiêm túc*

Tớ vừa kiểm tra đoạn clip bằng một công cụ phát hiện nội dung AI.

**MINH**

Kết quả cho thấy clip có 86% khả năng đã bị chỉnh sửa.

**LINH**

Vậy là deepfake rồi đúng không? Chúng ta có thể công bố rằng Mai An chưa từng nói câu đó.

## **S0.0.3Q — Có thể kết luận từ AI detector không?**

**Người chơi chọn một trong ba đáp án:**

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Đúng. 86% là đủ cao để kết luận đây là deepfake.”** | Evidence −2 Transparency −1 Minh Trust \+1 Crisis \+1 | MINH: “Kết quả cao không có nghĩa công cụ chắc chắn đúng. Video bị nén hoặc đăng lại nhiều lần cũng có thể làm sai kết quả.” Sau đó → S00\_MERGE\_02 |
| **B** | **“Chưa thể kết luận. Công cụ chỉ cung cấp một dấu hiệu để điều tra tiếp.”** | Evidence \+2 Transparency \+1 Minh Trust \+1 | MINH: “Chính xác. Nó có thể giúp mình đặt câu hỏi, nhưng không thể thay thế bằng chứng.” Sau đó → S00\_MERGE\_02 |
| **C** | **“AI detector thường không đáng tin, nên kết quả này hoàn toàn vô dụng.”** | Evidence −1 Minh Trust −2 | MINH: “Không nên tin tuyệt đối vào công cụ. Nhưng bỏ qua hoàn toàn một manh mối cũng không phải là xác minh.” Sau đó → S00\_MERGE\_02 |

 

## **S0.0.4 — Sự thật được tiết lộ**

**MINH**

Tớ đã tìm được bản phỏng vấn đầy đủ.

**MINH**

Trong đó, Mai An nói: “Không thể nói rằng người ở An Phúc không hiểu nghệ thuật. Vấn đề là chúng ta chưa tạo đủ cơ hội để họ tiếp cận.”

**LINH** *— bất ngờ*

Vậy từng từ trong clip đều là giọng thật của cô ấy. Nhưng người đăng đã cắt bỏ phần đầu câu.

**MINH**

Chưa hết. Biểu cảm trên gương mặt cô ấy cũng đã bị chỉnh sửa để trông khinh thường hơn.

**LINH**

Vậy chúng ta nên nói với cộng đồng thế nào?

## **S0.0.5Q — Đội truyền thông nên phản hồi thế nào?**

**Người chơi chọn một trong ba đáp án:**

| Lựa chọn | Người chơi chọn | Điểm thay đổi | Điều xảy ra ngay sau đó |
| :---- | :---- | :---- | :---- |
| **A** | **“Clip hoàn toàn giả. Mai An không hề nói câu đó.”** | Evidence −1 Transparency −2 Crisis \+1 | MINH: “Nhưng cô ấy thực sự đã nói những từ đó. Điều bị thao túng là ngữ cảnh và cách chúng được trình bày.” Hậu quả: công chúng tìm thấy bản gốc và cáo buộc ban tổ chức nói dối. Crisis \+1 Sau đó → S00\_END |
| **B** | **“Clip dùng lời nói thật nhưng đã cắt ngữ cảnh và chỉnh sửa hình ảnh để thay đổi ý nghĩa.”** | Evidence \+2 Transparency \+2 Crisis −2 | MINH: “Cách giải thích này chính xác với bằng chứng chúng ta đang có.” Hậu quả: bản đầy đủ được công bố; lượng chia sẻ clip gây hiểu lầm bắt đầu giảm. Crisis −1, Linh Trust \+1 Sau đó → S00\_END |
| **C** | **“Không cần phản hồi. Càng nói thì clip càng được chú ý.”** | Privacy \+1 Transparency −2 Crisis \+2 | LINH: “Nhưng hiện tại, bài đăng sai đang là nguồn thông tin duy nhất mà mọi người nhìn thấy.” Hậu quả: trong khoảng trống thông tin, các cáo buộc mới tiếp tục xuất hiện. Crisis \+1 Sau đó → S00\_END |

 

## **S0.0.6 — Kết thúc Scene 0**

**CASE FILE 01 — TRUE CONTENT, FALSE IMPRESSION**

**Bài học:** Nội dung không cần được tạo hoàn toàn bằng AI để gây hiểu lầm. Một câu nói thật có thể trở thành misinformation khi bị cắt khỏi ngữ cảnh, ghép với caption định hướng hoặc chỉnh sửa hình ảnh.

**Câu hỏi cần nhớ:** Nội dung trước và sau đoạn trích là gì?

**LINH** *— lo lắng*

Có người vừa gửi cho tớ một bức ảnh nhạy cảm được cho là của Vy, trưởng nhóm tình nguyện viên.

**MINH**

Và mọi người đang dùng nó để chứng minh rằng Vy có quan hệ với một thành viên ban tổ chức.

**NEXT SCENE: S01\_THE\_LEAKED\_IMAGE**

 

# **3\. Bảng node dành cho team dev**

| Node | Loại | Nội dung chính | Next |
| :---- | :---- | :---- | :---- |
| S00\_01 | Dialogue | Giới thiệu clip 8 giây và Linh hỏi nên xử lý thế nào. | CHOICE\_0\_1 |
| CHOICE\_0\_1\_A/B/C | Choice | Đăng lại ngay / chờ xác minh / cảnh báo mà không đăng lại clip. | S00\_MERGE\_01 |
| S00\_MERGE\_01 | Dialogue | Minh đưa kết quả AI detector 86%. | CHOICE\_0\_2 |
| CHOICE\_0\_2\_A/B/C | Choice | Ba cách diễn giải kết quả công cụ. | S00\_MERGE\_02 |
| S00\_MERGE\_02 | Dialogue | Bản phỏng vấn đầy đủ được tiết lộ. | CHOICE\_0\_3 |
| CHOICE\_0\_3\_A/B/C | Choice | Ba cách phản hồi công chúng và hậu quả. | S00\_END |
| S00\_END | Dialogue/System | Learning card ngắn và teaser cho ảnh của Vy. | S01\_THE\_LEAKED\_IMAGE |

Cấu trúc code: mọi response node của A/B/C đều đặt next về cùng một merge node. Không tô màu đáp án đúng trong UI hoặc tài liệu gửi người chơi.

