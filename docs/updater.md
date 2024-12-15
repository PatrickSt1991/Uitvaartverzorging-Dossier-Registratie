---
title: 99. Maintenance mode
layout: default
nav_enabled: true
nav_order: 99
---

Je kunt de applicatie in maintenance mode zetten, mits dit ingesteld staat bij Beheer -> Instellingen

De applicatie polt een web adres waar een xml bestand staat;

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
  <maintenance>false</maintenance>
  <version>4.0.8.5</version>
  <url>http://127.0.0.1:234/DossierRegistratie.zip</url>
</item>
```

verander de value van maintenance naar true, de volgende keer dat de applicatie de url polt worden de interface of in maintence gezet en kunnen de gebruikers niet meer inloggen, op deze manier kan er veilig een update gedaan worden.

