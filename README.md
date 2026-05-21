# ProjectOperationsDashboard


Requirement: 

สร้าง Abstract Base Class: NotificationChannelBase

สร้าง Channel 3 ตัวที่ Inherit จาก Base

สร้าง Interface: INotificationChannel (สร้าง interface INotificationChannel ที่กำหนด contract ว่าทุก Channel ต้องทำได้)

สร้าง NotificationService ที่รับ Channel หลายตัว

เพิ่ม Static Counter: TotalSentAllChannels

เพิ่ม RateLimiter (ใช้จำกัดว่า Channel ส่งได้สูงสุดกี่ครั้งต่อนาที) และตัดสินใจว่าจะวางไว้ที่ไหน


สร้าง PriorityNotificationService (สร้าง class PriorityNotificationService ที่ inherit จาก NotificationService และ 
override Broadcast() ให้รองรับ priority - เช่น ถ้า message เริ่มต้นด้วย [CRITICAL] ให้ส่งทุก channel, ถ้าเป็น [INFO] ให้ส่งแค่บาง channel เท่านั้น)

final output
<img width="550" height="481" alt="Screenshot 2026-05-15 165226" src="https://github.com/user-attachments/assets/335f78b5-9609-474e-9f1a-e149dabcafc4" />
