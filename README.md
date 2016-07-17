# Eventsourcing & CQRS demo: Hotel reservations.

Sample implementation of an aggregate and read model from the "Online Reservations" bounded context we identified in http://www.meetup.com/DDD-CQRS-ES/events/232482868/ .

## REST API:

### Create a room type (Double, Single, ...) and the number of units available:
'''
curl -X POST -H "Cache-Control: no-cache" "Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW" -F "name=Double" -F "NoOfUnits=3" "http://127.0.0.1:9000/rooms/types"
```

### Get a list of all room types
``
curl -X GET -H "Cache-Control: no-cache" "http://127.0.0.1:9000/rooms/types"
'''


