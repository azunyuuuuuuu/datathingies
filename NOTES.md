split the csv into countries via bash

```bash
wget https://raw.githubusercontent.com/owid/covid-19-data/master/public/data/owid-covid-data.csv
for country in `tail -n +2 owid-covid-data.csv | cut -d',' -f1 | sort | uniq`; do
    head -n1 owid-covid-data.csv > $country.csv;
    cat owid-covid-data.csv | grep $country >> $country.csv;
done
cat owid-covid-data.csv | cut -d',' -f1,2,3 | uniq > index.csv
```

## Generate `index.json` with jq

```bash
cat owid-covid-data.json | jq '[ to_entries | map_values(.value + { iso_code: .key }) | .[] | { iso_code, continent, location, file: "\(.iso_code).json" } ]' > index.json
```

sample output:
```json
[
  {
    "iso_code": "AFG",
    "continent": "Asia",
    "location": "Afghanistan",
    "file": "AFG.json"
  },
  {
    "iso_code": "DEU",
    "continent": "Europe",
    "location": "Germany",
    "file": "DEU.json"
  }
]
```

## Generate all country data with jq

```bash
for country in `cat index.json | jq --raw-output '.[].iso_code'`; do  cat owid-covid-data.json | jq .$country > $country.json; done
```