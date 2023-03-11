# Peachpie.Windows

This project was created to include functions that would need the COM library, not present in Peachpie.

## How to use

Include this project in your Peachpie solution, and add the reference to your project (*the one with the `index.php` file*).

Once that's done, just use it like this:

```php
<?php

$wmi = new Pchpie\Windows\WMI();

$cpu_cores = $wmi->GetCPUCores();
$physical_memory = $wmi->GetPhysicalMemory();
$load = $wmi->GetLoad();

echo "CPU cores: " . $cpu_cores . "<br>";
echo "Physical memory" . "<br>";
print_r($physical_memory);
echo "<br>";
echo "System load" . "<br>";
print_r($load);
```


