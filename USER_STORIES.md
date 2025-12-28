# PocketMechanic - User Stories & Development Flow

## Product Vision
A local-first mobile app that helps car owners track their vehicle maintenance tasks, focusing on parts and services they can handle themselves without a mechanic.

## Core Principles
- **100% Local Storage**: No cloud sync, all data stored on device
- **Privacy First**: No user data collection or transmission
- **DIY Focused**: Track maintenance items manageable by car owners

---

## User Stories

### Epic 1: Vehicle Management

#### US-1.1: Add Vehicle to Garage
**As a** car owner  
**I want to** add my vehicle to the app  
**So that** I can start tracking its maintenance

**Acceptance Criteria:**
- User can tap "Add Vehicle" button
- Form includes fields:
  - Make (text)
  - Model (text)
  - Year (number)
  - VIN (optional, text)
  - License Plate (optional, text)
  - Mileage (number)
  - Purchase Date (optional, date)
  - Nickname (optional, text)
- Data is saved locally to device storage
- Vehicle appears in "My Garage" list

**Technical Notes:**
- Use SQLite for local storage
- Model: `Vehicle` class with properties

---

#### US-1.2: View My Garage
**As a** car owner  
**I want to** see all my vehicles in a list  
**So that** I can select which vehicle to manage

**Acceptance Criteria:**
- Main page displays all saved vehicles
- Each vehicle card shows: nickname/model, year, current mileage
- Tap on vehicle opens vehicle detail page
- Empty state shows "Add your first vehicle" prompt

---

#### US-1.3: Edit Vehicle Information
**As a** car owner  
**I want to** update my vehicle information  
**So that** I can keep details current (especially mileage)

**Acceptance Criteria:**
- User can access edit mode from vehicle detail page
- All vehicle fields are editable
- Save button persists changes locally
- Cancel button discards changes

---

#### US-1.4: Delete Vehicle
**As a** car owner  
**I want to** remove a vehicle I no longer own  
**So that** my garage only shows current vehicles

**Acceptance Criteria:**
- Delete option available in vehicle detail page
- Confirmation dialog appears before deletion
- Deleting vehicle also deletes all associated maintenance records
- User returns to garage list after deletion

---

### Epic 2: Maintenance Tracking

#### US-2.1: Add Oil Change Record
**As a** car owner  
**I want to** record when I changed my oil  
**So that** I know when the next change is due

**Acceptance Criteria:**
- User can add maintenance record from vehicle detail page
- Form includes:
  - Maintenance Type: "Oil Change"
  - Date Performed (date picker, defaults to today)
  - Mileage at Service (number)
  - Next Due Date (optional, date picker)
  - Next Due Mileage (optional, number)
  - Notes (optional, text)
  - Cost (optional, number)
- Record is saved and linked to vehicle

---

#### US-2.2: Track Common Maintenance Items
**As a** car owner  
**I want to** track various DIY maintenance tasks  
**So that** I can stay on top of all vehicle care

**Maintenance Types Supported:**
- Oil Change (interval: 5,000-7,500 miles or 6 months)
- Oil Filter (interval: with oil change)
- Air Filter (interval: 15,000-30,000 miles or 12 months)
- Cabin Air Filter (interval: 15,000-25,000 miles or 12 months)
- Tire Rotation (interval: 5,000-7,500 miles or 6 months)
- Wiper Blades (interval: 6-12 months)
- Battery (interval: 3-5 years)
- Spark Plugs (interval: 30,000-100,000 miles depending on type)
- Coolant Flush (interval: 30,000 miles or 2 years)
- Brake Fluid (interval: 2-3 years)
- Power Steering Fluid (interval: check annually)
- Transmission Fluid (interval: 30,000-60,000 miles)

**Acceptance Criteria:**
- User can select from predefined maintenance types
- Each type has suggested interval (user can customize)
- App calculates next due date based on mileage or time

---

#### US-2.3: View Maintenance History
**As a** car owner  
**I want to** see all past maintenance for a vehicle  
**So that** I can track what's been done

**Acceptance Criteria:**
- Vehicle detail page shows list of all maintenance records
- Records sorted by date (most recent first)
- Each record shows: type, date, mileage, next due indicator
- Tap on record to view full details

---

#### US-2.4: Get Maintenance Reminders
**As a** car owner  
**I want to** see which maintenance items are due soon  
**So that** I can plan my vehicle care

**Acceptance Criteria:**
- Dashboard shows overdue items (red indicator)
- Dashboard shows items due soon (yellow indicator, within 500 miles or 30 days)
- Dashboard shows upcoming items (green, all good)
- Badge count on vehicle card shows overdue/due soon count

---

#### US-2.5: Edit Maintenance Record
**As a** car owner  
**I want to** update a maintenance record  
**So that** I can correct mistakes or update information

**Acceptance Criteria:**
- User can tap on existing record to edit
- All fields are editable
- Save persists changes locally
- Cancel discards changes

---

#### US-2.6: Delete Maintenance Record
**As a** car owner  
**I want to** remove an incorrect maintenance record  
**So that** my history is accurate

**Acceptance Criteria:**
- Delete option in record detail view
- Confirmation dialog before deletion
- Record is permanently removed

---

### Epic 3: Smart Features

#### US-3.1: Custom Maintenance Items
**As a** car owner  
**I want to** add custom maintenance items  
**So that** I can track vehicle-specific needs

**Acceptance Criteria:**
- User can create custom maintenance type
- Custom type includes: name, default interval (mileage/time)
- Custom types appear alongside predefined types
- Custom types can be edited or deleted

---

#### US-3.2: Interval-Based Due Date Calculation
**As a** car owner  
**I want to** the app to calculate next due date automatically  
**So that** I don't have to do math

**Acceptance Criteria:**
- When user logs maintenance, app suggests next due based on:
  - Current mileage + typical interval
  - Current date + typical time interval
- User can accept suggestion or enter custom value
- App shows which comes first (mileage or date)

---

#### US-3.3: Update Current Mileage
**As a** car owner  
**I want to** quickly update my current vehicle mileage  
**So that** due date calculations are accurate

**Acceptance Criteria:**
- Quick update button on vehicle card
- Simple input for current mileage
- Updates vehicle record
- Refreshes maintenance due status

---

## Technical Architecture

### Data Models

```csharp
public class Vehicle
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string VIN { get; set; }
    public string LicensePlate { get; set; }
    public int CurrentMileage { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string Nickname { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public List<MaintenanceRecord> MaintenanceRecords { get; set; }
}

public class MaintenanceRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string MaintenanceType { get; set; }
    public DateTime DatePerformed { get; set; }
    public int MileageAtService { get; set; }
    public DateTime? NextDueDate { get; set; }
    public int? NextDueMileage { get; set; }
    public string Notes { get; set; }
    public decimal? Cost { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public Vehicle Vehicle { get; set; }
}

public class MaintenanceType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? DefaultMileageInterval { get; set; }
    public int? DefaultMonthsInterval { get; set; }
    public bool IsCustom { get; set; }
    public string Description { get; set; }
}
```

### Technology Stack
- **.NET MAUI**: Cross-platform UI framework
- **SQLite**: Local database storage
- **CommunityToolkit.MVVM**: MVVM pattern implementation
- **CommunityToolkit.Maui**: Additional MAUI controls

### Application Flow

```
App Start
  ↓
Main Page (My Garage)
  ├─ Empty State → [Add Vehicle Button]
  ├─ Vehicle List
  │   └─ Vehicle Card (shows overdue count badge)
  │       ↓ [Tap]
  │       Vehicle Detail Page
  │         ├─ Vehicle Info Section
  │         │   └─ [Edit] [Update Mileage] [Delete]
  │         ├─ Due Soon Section
  │         │   └─ Overdue/Due items listed
  │         ├─ Maintenance History Section
  │         │   └─ All records (sorted by date)
  │         │       ↓ [Tap]
  │         │       Maintenance Detail Page
  │         │         └─ [Edit] [Delete]
  │         └─ [Add Maintenance Button]
  │             ↓
  │             Add/Edit Maintenance Page
  │               ├─ Select Type (predefined or custom)
  │               ├─ Date Performed
  │               ├─ Mileage
  │               ├─ Next Due (auto-calculated)
  │               ├─ Notes
  │               ├─ Cost
  │               └─ [Save] [Cancel]
  └─ [Add Vehicle Button]
      ↓
      Add Vehicle Page
        ├─ Vehicle Information Form
        └─ [Save] [Cancel]
```

### Storage Strategy
- **Local SQLite Database**: Primary storage
- **Location**: App's local data directory
- **No Cloud Sync**: All data remains on device
- **Backup**: User responsible (via device backup solutions)

---

## Development Phases

### Phase 1: Foundation (MVP)
- [ ] Set up SQLite database
- [ ] Create Vehicle model and CRUD operations
- [ ] Create MaintenanceRecord model and CRUD operations
- [ ] Build "My Garage" page (vehicle list)
- [ ] Build "Add Vehicle" page
- [ ] Build "Vehicle Detail" page

### Phase 2: Core Maintenance Tracking
- [ ] Build "Add Maintenance" page
- [ ] Implement predefined maintenance types
- [ ] Add maintenance history view
- [ ] Implement due date calculations
- [ ] Add overdue/due soon indicators

### Phase 3: Enhanced Features
- [ ] Add custom maintenance types
- [ ] Implement quick mileage update
- [ ] Add cost tracking and reporting
- [ ] Improve UI/UX with animations
- [ ] Add data export functionality (CSV)

### Phase 4: Polish
- [ ] Add onboarding/tutorial
- [ ] Implement search/filter for maintenance records
- [ ] Add statistics/insights (total spent, items completed)
- [ ] Performance optimization
- [ ] Comprehensive testing

---

## Design Considerations

### UI/UX Principles
- **Simple & Clean**: Minimal learning curve
- **Touch-Friendly**: Large tap targets for mobile
- **Clear Status Indicators**: Color-coded due status (red/yellow/green)
- **Quick Actions**: Common tasks accessible in 1-2 taps

### Accessibility
- Support for screen readers
- High contrast mode support
- Scalable font sizes
- Clear visual feedback for actions

### Performance
- Fast app startup
- Smooth scrolling for large lists
- Efficient database queries
- Minimal battery usage
