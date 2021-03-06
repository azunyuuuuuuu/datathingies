split the csv into countries via bash

```bash
for country in `tail -n +2 owid-covid-data.csv | cut -d',' -f1 | sort | uniq`; do
    head -n1 owid-covid-data.csv > $country.csv;
    cat owid-covid-data.csv | grep $country >> $country.csv;
done
cat owid-covid-data.csv | cut -d',' -f1,3 | uniq > index.csv
```
