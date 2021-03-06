split the csv into countries via bash

```bash
wget https://raw.githubusercontent.com/owid/covid-19-data/master/public/data/owid-covid-data.csv
for country in `tail -n +2 owid-covid-data.csv | cut -d',' -f1 | sort | uniq`; do
    head -n1 owid-covid-data.csv > $country.csv;
    cat owid-covid-data.csv | grep $country >> $country.csv;
done
cat owid-covid-data.csv | cut -d',' -f1,3 | uniq > index.csv
```
