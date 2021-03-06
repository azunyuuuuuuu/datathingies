split the csv into countries via bash

```bash
for country in `cat owid-covid-data.json | jq --raw-output 'keys[]'`; do
    head -n1 owid-covid-data.csv > $country.csv;
    cat owid-covid-data.csv | grep $country >> $country.csv;
done
```