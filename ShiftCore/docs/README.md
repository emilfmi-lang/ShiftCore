# ShiftCore — Günd?lik Davamiyy?t Sistemi

## M?qs?d

Tikinti sah?sind? (v? ya ox?ar i? yerl?rind?) **günd?lik davamiyy?t v?r?qini** r?q?msal formada idar? etm?k.
Sistem **database istifad? etmir** — bütün m?lumatlar JSON fayllar?nda saxlan?l?r v? n?tic? **Excel (.xlsx)** format?nda ixrac olunur.

??kild?ki ka??z davamiyy?t v?r?qi ("Günd?lik Davamiyy?t V?r?qi") bu sistemin ?sas modelidir.

---

## Nec? ??l?yir (Ümumi Ax?n)

```
1. Admin i?çil?ri ?lav? edir (ad, soyad, v?zif?)
        ?
2. ??çi s?h?r giri? edir ? vaxt + imza qeyd olunur
        ?
3. ??çi ax?am ç?x?? edir ? vaxt + imza qeyd olunur
        ?
4. Günd?lik davamiyy?t Excel-? ixrac edilir
```

---

## Layih? Strukturu

```
ShiftCore/
??? Controllers/
?   ??? WorkerController.cs        # ??çi CRUD ?m?liyyatlar? (API)
?   ??? AttendanceController.cs    # Giri?/Ç?x?? qeydiyyat? + Excel ixrac (API)
?
??? Entity/
?   ??? Common/
?   ?   ??? BaseEntity.cs          # Baza entity (Id: Guid)
?   ??? Worker.cs                  # ??çi modeli
?   ??? AttendanceRecord.cs        # Davamiyy?t qeydi modeli
?
??? Services/
?   ??? WorkerService.cs           # ??çi biznes m?ntiqi
?   ??? AttendanceService.cs       # Davamiyy?t biznes m?ntiqi
?
??? Infrastructure/
?   ??? JsonStorage.cs             # JSON faylda oxu/yaz (database ?v?zi)
?   ??? ExcelExporter.cs           # Excel format?nda davamiyy?t v?r?qi yarad?r
?
??? Data/
?   ??? workers.json               # ??çil?r siyah?s? (runtime-da yaran?r)
?   ??? attendance.json            # Davamiyy?t qeydl?ri (runtime-da yaran?r)
?
??? Program.cs                     # DI konfiqurasiya + API pipeline
```

---

## Entity Modell?ri

### BaseEntity
| Sah? | Tip    | Aç?qlama                  |
|------|--------|---------------------------|
| Id   | `Guid` | H?r qeydin unikal ID-si   |

### Worker
| Sah?        | Tip        | Aç?qlama                                  |
|-------------|------------|-------------------------------------------|
| FullName    | `string`   | ??çinin ad?, soyad?, atas?n?n ad?         |
| Role        | `string`   | V?zif? (Memar, Usta, F?hl?, Formen, ...) |
| IsActive    | `bool`     | Aktiv/deaktiv (silinm? ?v?zin?)           |
| CreatedAt   | `DateTime` | Yaranma tarixi                             |

### AttendanceRecord
| Sah?           | Tip         | Aç?qlama                        |
|----------------|-------------|---------------------------------|
| WorkerId       | `Guid`      | Hans? i?çiy? aiddir              |
| Date           | `DateTime`  | Tarix (gün)                     |
| EntryTime      | `DateTime?` | Giri? vaxt?                     |
| EntrySignature | `string?`   | Giri? imzas?                    |
| ExitTime       | `DateTime?` | Ç?x?? vaxt?                     |
| ExitSignature  | `string?`   | Ç?x?? imzas?                    |

> **Qeyd:** `AttendanceRecord`-a `EntrySignature` v? `ExitSignature` sah?l?ri ?lav? edilm?lidir.

---

## API Endpointl?ri (Plan)

### WorkerController — `api/worker`

| Metod    | Route              | Aç?qlama                 |
|----------|--------------------|--------------------------|
| `GET`    | `/api/worker`      | Bütün aktiv i?çil?r      |
| `GET`    | `/api/worker/{id}` | Bir i?çinin m?lumat?     |
| `POST`   | `/api/worker`      | Yeni i?çi ?lav? et       |
| `PUT`    | `/api/worker/{id}` | ??çi m?lumat?n? yenil?   |
| `DELETE` | `/api/worker/{id}` | ??çini deaktiv et        |

#### POST/PUT Request Body:
```json
{
  "fullName": "?flatun Soltanov",
  "role": "Usta"
}
```

---

### AttendanceController — `api/attendance`

| Metod   | Route                          | Aç?qlama                           |
|---------|--------------------------------|------------------------------------|
| `POST`  | `/api/attendance/checkin`      | ??çinin giri? qeydiyyat?          |
| `POST`  | `/api/attendance/checkout`     | ??çinin ç?x?? qeydiyyat?          |
| `GET`   | `/api/attendance/{date}`       | H?min günün bütün qeydl?ri         |
| `GET`   | `/api/attendance/export/{date}`| H?min günün Excel fayl? (download) |

#### CheckIn / CheckOut Request Body:
```json
{
  "workerId": "guid-buraya",
  "signature": "imza-m?tni"
}
```

---

## Biznes Qaydalar?

### 1. Giri? Qeydiyyat? (Check-In)
- H?r i?çi günd? **yaln?z bir d?f?** giri? ed? bil?r
- Giri? vaxt? v? imza avtomatik qeyd olunur

### 2. Ç?x?? Qeydiyyat? (Check-Out)
- Ç?x?? yaln?z giri? edildikd?n **sonra** mümkündür
- ? **Giri?d?n minimum 3 saat keçm?d?n ç?x?? etm?k olmaz**
- ?g?r 3 saat keçm?yibs?, qalan vaxt istifad?çiy? göst?rilir:
  > _"Giri?d?n ?n az? 3 saat keçm?lidir. H?l? 1 saat 45 d?qiq? qal?b."_

### 3. Excel ?xrac?
- H?r gün üçün "Günd?lik Davamiyy?t V?r?qi" yarad?l?r
- Format ??kild?ki ka??z v?r?q? uy?undur

---

## Excel Ç?xar?? Format? (Günd?lik Davamiyy?t V?r?qi)

A?a??dak? format ??kild?ki ka??z v?r?q? ?saslan?r:

```
?????????????????????????????????????????????????????????????????
?              GÜND?L?K DAVAM?YY?T V?R?Q?                    ?
?                                    GÜN: 04  AY: 03  ?L: 2026?
????????????????????????????????????????????????????????????????
? S/S? Ad, soyad            ? V?zif?si  ? Giri?? ?mza?Ç?x????mza?
????????????????????????????????????????????????????????????????
?  1 ? Murat Güler          ? Memar     ? 08:00?  ?  ?18:00? ? ?
?  2 ? Yusif Yusifov        ? Sah? R?isi? 08:00?  ?  ?18:00? ? ?
?  3 ? ?flatun Soltanov     ? Usta      ? 08:00?  ?  ?18:00? ? ?
?  4 ? Taleh Bayramov       ? F?hl?    ? 08:00?  ?  ?12:00? ? ?
? ...? ...                  ? ...       ? ...  ? ... ? ... ?...?
????????????????????????????????????????????????????????????????
```

### S?tir r?ngl?ri (role-a gör?):
| V?zif?          | R?ng        |
|------------------|-------------|
| Usta, F?hl?      | Aç?q sar?   |
| Lesa             | Sar?        |
| Dig?r (Memar, Formen, ...) | R?ngsiz |

---

## Texnoloji Stack

| Komponent       | Texnologiya                          |
|-----------------|--------------------------------------|
| Framework       | .NET 10 (ASP.NET Core Web API)       |
| Dil             | C# 14                                |
| M?lumat saxlama | JSON fayllar (`Data/` qovlu?u)       |
| Excel ixrac     | EPPlus 7.7.2                         |
| API Docs        | OpenAPI + Scalar                     |

---

## JsonStorage — Nec? ??l?yir

Database ?v?zin? `Data/` qovlu?unda JSON fayllar istifad? olunur:

- `workers.json` — bütün i?çil?rin siyah?s?
- `attendance.json` — bütün davamiyy?t qeydl?ri

`JsonStorage` sinfi generic `LoadAsync<T>` v? `SaveAsync<T>` metodlar? il? ist?nil?n entity-ni fayla yaza / fayldan oxuya bilir.

> **Niy? database yoxdur?** Layih? kiçik miqyasl?d?r, tikinti sah?sind?ki bir komanda üçündür. JSON fayllar sad?, portativ v? heç bir server/infra t?l?b etmir.

---

## Servis Layeri — Daxili M?ntiq (Detall?)

---

### WorkerService

> **As?l?l?q:** `JsonStorage` (constructor injection il?)
> **Fayl:** `Data/workers.json`

---

#### `GetAllAsync()` ? `List<Worker>`

Bütün aktiv i?çil?ri qaytar?r.

```
Add?mlar:
1. JsonStorage.LoadAsync<Worker>("workers.json") — bütün i?çil?ri oxu
2. Siyah?n? filtr et ? yaln?z IsActive == true olanlar
3. OrderIndex-? gör? s?rala (kiçikd?n böyüy?)
4. N?tic?ni List<Worker> olaraq qaytar
```

---

#### `GetByIdAsync(Guid id)` ? `Worker?`

ID il? bir i?çini tap?r.

```
Add?mlar:
1. JsonStorage.LoadAsync<Worker>("workers.json") — bütün i?çil?ri oxu
2. Siyah?da FirstOrDefault il? tap ? Id == id V? IsActive == true
3. Tap?lsa ? Worker qaytar, tap?lmasa ? null qaytar
```

---

#### `AddAsync(string fullName, string role)` ? `Worker`

Yeni i?çi ?lav? edir.

```
Add?mlar:
1. JsonStorage.LoadAsync<Worker>("workers.json") — mövcud siyah?n? oxu
2. Mövcud siyah?da ?n böyük OrderIndex-i tap (siyah? bo?dursa ? 0)
3. Yeni Worker obyekti yarat:
   - FullName = fullName
   - Role = role
   - OrderIndex = maxOrderIndex + 1
   - IsActive = true (default)
   - CreatedAt = DateTime.UtcNow (default)
   - Id = Guid.NewGuid() (BaseEntity-d?n g?lir)
4. Yeni worker-i siyah?ya ?lav? et
5. JsonStorage.SaveAsync("workers.json", siyah?) — fayla yaz
6. Yarad?lm?? Worker-i qaytar
```

---

#### `UpdateAsync(Guid id, string fullName, string role)` ? `bool`

Mövcud i?çinin m?lumat?n? yenil?yir.

```
Add?mlar:
1. JsonStorage.LoadAsync<Worker>("workers.json") — siyah?n? oxu
2. Siyah?da Id == id olan worker-i tap
3. Tap?lmasa ? false qaytar
4. Tap?lsa:
   - worker.FullName = fullName
   - worker.Role = role
5. JsonStorage.SaveAsync("workers.json", siyah?) — yenil?nmi? siyah?n? fayla yaz
6. true qaytar
```

---

#### `DeactivateAsync(Guid id)` ? `bool`

??çini deaktiv edir (soft delete — silinmir, IsActive = false olur).

```
Add?mlar:
1. JsonStorage.LoadAsync<Worker>("workers.json") — siyah?n? oxu
2. Siyah?da Id == id olan worker-i tap
3. Tap?lmasa ? false qaytar
4. Tap?lsa:
   - worker.IsActive = false
5. JsonStorage.SaveAsync("workers.json", siyah?) — fayla yaz
6. true qaytar
```

> **Qeyd:** Heç vaxt fiziki silm? etmirik. Deaktiv edilmi? i?çi JSON-da qal?r amma
> `GetAllAsync()` onu qaytarm?r çünki `IsActive == true` filtri var.

---

### AttendanceService

> **As?l?l?q:** `JsonStorage`, `ExcelExporter`, `WorkerService` (constructor injection il?)
> **Fayl:** `Data/attendance.json`
> **Sabit:** `MinimumWorkDuration = 3 saat`

---

#### `CheckInAsync(Guid workerId, string signature)` ? `AttendanceRecord`

??çinin giri? qeydiyyat?n? apar?r.

```
Add?mlar:
1. JsonStorage.LoadAsync<AttendanceRecord>("attendance.json") — qeydl?ri oxu
2. Bu günün tarixini al ? today = DateTime.Today
3. Siyah?da axtar ? WorkerId == workerId V? Date == today
4. ?G?R tap?l?b V? EntryTime art?q dolu ?
   ? InvalidOperationException: "Bu i?çi art?q bu gün giri? qeydiyyat?ndan keçib."
5. ?G?R tap?l?b amma EntryTime bo?dur ?
   - record.EntryTime = DateTime.Now
   - record.EntrySignature = signature
6. ?G?R tap?lmay?b ?
   - Yeni AttendanceRecord yarat:
     - WorkerId = workerId
     - Date = today
     - EntryTime = DateTime.Now
     - EntrySignature = signature
   - Siyah?ya ?lav? et
7. JsonStorage.SaveAsync("attendance.json", siyah?) — fayla yaz
8. Qeydi qaytar
```

---

#### `CheckOutAsync(Guid workerId, string signature)` ? `AttendanceRecord`

??çinin ç?x?? qeydiyyat?n? apar?r. **3 saat qaydas? burada t?tbiq olunur.**

```
Add?mlar:
1. JsonStorage.LoadAsync<AttendanceRecord>("attendance.json") — qeydl?ri oxu
2. Bu günün tarixini al ? today = DateTime.Today
3. Siyah?da axtar ? WorkerId == workerId V? Date == today
4. ?G?R tap?lmay?b V? YA EntryTime bo?dur ?
   ? InvalidOperationException: "Bu i?çi h?l? giri? qeydiyyat?ndan keçm?yib."
5. ?G?R ExitTime art?q doludur ?
   ? InvalidOperationException: "Bu i?çi art?q ç?x?? qeydiyyat?ndan keçib."
6. ??l?nmi? müdd?ti hesabla:
   - workedDuration = DateTime.Now - record.EntryTime
7. ?G?R workedDuration < 3 saat ?
   - remaining = 3 saat - workedDuration
   ? InvalidOperationException: 
     "Giri?d?n ?n az? 3 saat keçm?lidir. H?l? {remaining.Hours} saat {remaining.Minutes} d?qiq? qal?b."
8. ?G?R 3 saat keçib ?
   - record.ExitTime = DateTime.Now
   - record.ExitSignature = signature
9. JsonStorage.SaveAsync("attendance.json", siyah?) — fayla yaz
10. Qeydi qaytar
```

---

#### `GetByDateAsync(DateTime date)` ? `List<AttendanceRecord>`

Verilmi? tarix? aid bütün davamiyy?t qeydl?rini qaytar?r.

```
Add?mlar:
1. JsonStorage.LoadAsync<AttendanceRecord>("attendance.json") — qeydl?ri oxu
2. Filtr et ? yaln?z Date.Date == date.Date olanlar
3. N?tic?ni List olaraq qaytar
```

---

#### `ExportDailyExcelAsync(DateTime date)` ? `byte[]`

Verilmi? gün üçün Excel fayl?n? generasiya edir.

```
Add?mlar:
1. WorkerService.GetAllAsync() — aktiv i?çil?r siyah?s?n? al
2. this.GetByDateAsync(date) — h?min günün qeydl?rini al
3. ExcelExporter.GenerateDailyAttendanceSheet(date, workers, records) — Excel yarat
4. byte[] olaraq qaytar (controller File() il? endir?c?k)
```

---

### JsonStorage — Daxili M?ntiq

> **As?l?l?q:** `IWebHostEnvironment` (ContentRootPath almaq üçün)
> **Qovluq:** `{ContentRootPath}/Data/`

---

#### `LoadAsync<T>(string fileName)` ? `List<T>`

```
Add?mlar:
1. Tam yolu hesabla ? Path.Combine(dataDirectory, fileName)
2. ?G?R fayl mövcud deyil ? bo? List<T> qaytar
3. Fayl?n m?zmununu oxu ? File.ReadAllTextAsync(path)
4. ?G?R m?zmun bo?dur ? bo? List<T> qaytar
5. JSON-u deserialize et ? JsonSerializer.Deserialize<List<T>>(json)
6. N?tic?ni qaytar (null olsa ? bo? siyah?)
```

#### `SaveAsync<T>(string fileName, List<T> data)` ? `Task`

```
Add?mlar:
1. Tam yolu hesabla ? Path.Combine(dataDirectory, fileName)
2. Data-n? JSON-a serialize et ? JsonSerializer.Serialize(data) (WriteIndented = true)
3. Fayla yaz ? File.WriteAllTextAsync(path, json)
```

---

### ExcelExporter — Daxili M?ntiq

#### `GenerateDailyAttendanceSheet(DateTime date, List<Worker> workers, List<AttendanceRecord> records)` ? `byte[]`

```
Add?mlar:
1. EPPlus lisenziyas?n? t?yin et (NonCommercial)
2. Yeni ExcelPackage yarat
3. Worksheet ?lav? et ? ad: "Davamiyy?t"

4. BA?LIQ (Row 1):
   - A1:G1 birl??dir (merge)
   - Yaz: "GÜND?L?K DAVAM?YY?T V?R?Q?"
   - Font: Bold, Size 14, ortada (center)

5. TAR?X (Row 2-3):
   - E2: "GÜN"   F2: "AY"   G2: "?L"
   - E3: date.Day  F3: date.Month  G3: date.Year

6. KATEQOR?YA (Row 4):
   - A4:C4 birl??dir ? "Müh?ndis hey?ti - Usta, F?hl?"

7. SÜTUN BA?LIQLAR (Row 5):
   - A5: "S/S"
   - B5: "Ad, soyad, atas?n?n ad?"
   - C5: "V?zif?si"
   - D5: "Giri?"
   - E5: "?mza"
   - F5: "Ç?x??"
   - G5: "?mza"
   - Stil: Bold, aç?q boz fon, alt x?tt

8. M?LUMAT S?T?RL?R? (Row 6+):
   - ??çil?ri OrderIndex-? gör? s?rala
   - H?r i?çi üçün:
     - A: s?ra nömr?si (1, 2, 3...)
     - B: worker.FullName
     - C: worker.Role
     - D: record.EntryTime?.ToString("HH:mm") (varsa)
     - E: record.EntrySignature (varsa)
     - F: record.ExitTime?.ToString("HH:mm") (varsa)
     - G: record.ExitSignature (varsa)
   - S?T?R R?NGL?R? (v?zif?y? gör?):
     - "F?hl?" v? ya "Usta" ? aç?q sar? (LightYellow)
     - "Lesa" ? sar? (Yellow)
     - Dig?r ? r?ngsiz

9. HA?IY?L?R:
   - Bütün data hüceyr?l?rin? ince (Thin) border ?lav? et

10. SÜTUN ENL?R?:
    - A: 5  |  B: 28  |  C: 18  |  D: 10  |  E: 14  |  F: 10  |  G: 14

11. package.GetAsByteArray() ? byte[] olaraq qaytar
```

---

## Program.cs — DI Konfiqurasiyas? (Plan)

```
Qeydiyyat olunmal? servisl?r:
??? JsonStorage      ? Singleton
??? ExcelExporter    ? Singleton
??? WorkerService    ? Scoped
??? AttendanceService ? Scoped

Middleware pipeline:
??? OpenAPI / Scalar (development)
??? MapControllers
```

---

## ?stifad? Ssenaris? (Nümun?)

```
S?h?r 08:00 — Sah? r?isi proqram? aç?r

1. ??çil?r siyah?s? ekranda görünür (GET /api/worker)

2. H?r i?çi öz ad?n?n yan?ndak? "Giri?" düym?sin? bas?r
   ? POST /api/attendance/checkin { workerId, signature }
   ? Saat avtomatik yaz?l?r (08:00, 08:05, ...)

3. Ax?am 17:00+ — i?çil?r "Ç?x??" düym?sin? bas?r
   ? POST /api/attendance/checkout { workerId, signature }
   ? ?g?r 3 saat keçm?yibs? ? x?ta mesaj?
   ? Keçibs? ? saat + imza yaz?l?r

4. Günün sonunda ? Excel yükl?nir
   ? GET /api/attendance/export/2026-03-04
   ? "Davamiyyet_2026-03-04.xlsx" fayl? endirilir
```

---

## Növb?ti Add?mlar (TODO)

- [ ] `AttendanceRecord`-a `EntrySignature` v? `ExitSignature` sah?l?ri ?lav? et
- [ ] `Worker`-? `OrderIndex` sah?si ?lav? et (s?ra nömr?si üçün)
- [ ] `JsonStorage` — `LoadAsync<T>` / `SaveAsync<T>` implementasiya et
- [ ] `ExcelExporter` — EPPlus il? ??kild?ki formata uy?un Excel yarad?lmas?
- [ ] `WorkerService` — CRUD ?m?liyyatlar? implementasiya et
- [ ] `AttendanceService` — giri?/ç?x?? m?ntiqi + 3 saat qaydas?
- [ ] `WorkerController` — endpoint-l?ri yaz
- [ ] `AttendanceController` — endpoint-l?ri yaz
- [ ] `Program.cs` — DI qeydiyyat? v? pipeline konfiqurasiyas?
- [ ] Frontend (g?l?c?kd?): sad? UI — i?çi siyah?s? + giri?/ç?x?? düym?l?ri
