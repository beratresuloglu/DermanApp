```mermaid
stateDiagram-v2
    [*] --> HelpRequestFlow

    state HelpRequestFlow {
        [*] --> Acik
        Acik --> OnayBekliyor: Yardimci Ustlenmek Ister
        OnayBekliyor --> Ustlenildi: Afetzede Onaylar
        OnayBekliyor --> Acik: Afetzede Reddeder
        Ustlenildi --> Tamamlandi: Yardim Teslim Edildi
        Tamamlandi --> [*]
    }

    [*] --> MatchFlow

    state MatchFlow {
        [*] --> Bekliyor
        Bekliyor --> Onaylandi: Afetzede Onaylar
        Bekliyor --> Reddedildi: Afetzede Reddeder
        Onaylandi --> Tamamlandi: Islem Tamamlanir
        Reddedildi --> [*]
        Tamamlandi --> [*]
    }

    [*] --> UserFlow

    state UserFlow {
        [*] --> Aktif
        Aktif --> Incelemede: 3+ Sikayet Alir
        Incelemede --> Aktif: Inceleme Sonucu Temiz
        Incelemede --> Engellendi: Inceleme Sonucu Ihlal Bulunur
        Engellendi --> [*]
    }
```
