[![License](https://img.shields.io/github/license/LucasRosinelli/CancunSurvivor)](./LICENSE)

# Cancun Survivor
People are now free to travel everywhere but because of the pandemic, a lot of hotels went bankrupt. Some former famous travel places are left with only one hotel.
You've been given the responsibility to develop a booking API for the very last hotel in *Cancun*.

## Requirements
- API will be maintained by the hotel's IT department.
- As it's the very last hotel, the quality of service must be 99.99 to 100% => no downtime
- For the purpose of the test, we assume the hotel has only one room available
- To give a chance to everyone to book the room, the stay can't be longer than **3 days** and can't be reserved more than 30 days in advance.
- All reservations start at least the next day of booking,
- To simplify the use case, a "*DAY*" in the hotel room starts from **00:00** to **23:59:59**.
- Every end-user can check the room availability, place a reservation, cancel it or modify it.
- To simplify the API is insecure.

## Instructions
- No time limit (very well done, you need at least 3 to 4 evenings)
- The minimum required is a README and code.
- All shortcuts to save time are allowed as far as documented. Any unexplained shortcut should be considered an error. We could accept a rendering with 3 lines of code if they make sense and all the reasoning and issues to be taken into account are described.

## Overview
The application was built in .NET 5 and uses an in-memory database to store the data.

## Usage
The API intends to follow common standards and practices. The endpoints are defined in plural and use HTTP methods to behave accordingly: **GET**, **POST**, **PUT** and **DELETE**.There are two areas available:

### Rooms
The management of the single room available.
- You can check all available rooms
  - GET /rooms
- You can obtain the room (without its reservations)
  - GET /rooms/{id}
    - where [id} is a GUID
- You can modify the name of the room
  - PUT /rooms/{id}
    - where [id} is a GUID
- You cannot add a new room
- You cannot delete the existing room

### Reservation
The management of the reservations of the room.
- You can check all existing reservations
  - GET /reservations
- You can obtain the reservation
  - GET /reservations/{id}
    - where [id} is a GUID
- You can add a new reservation
  - POST /reservations
- You can modify the reservation
  - PUT /reservations/{id}
    - where [id} is a GUID
    - you cannot change who made the reservation (customer email)
- You can delete an existing reservation
  - DELETE /reservations/{id}
    - where [id} is a GUID

Reservations validations follow the rules stated:
- Maximum of 3 days stay.
- Can book on day after (current date) and later.
- The last day of the stay cannot exceed 30 days from current date.
- The same customer (customer email) cannot book two reservations sequentially without a day between them.

### Notes
> - An in-memory database was chosen for this demo.
> - The single room is generated each time the application starts and its id is randomly generated. There is no need to worry about it as booking new reservations does not require to set it.
> - Local datetime was considered the base for comparison. In real-world scenarios, the timezones and UTC should take place and play an important role.
> - I intended to use a simplified version of _clean-architecture_. I'm studying it lately so it was an opportunity to practice more.

## Author
[Lucas Rosinelli](https://lucasrosinelli.com/)
