/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { connect, MapStateToProps } from 'react-redux';
import { List, Map, Set } from 'immutable';
import { ActivityIndicator, Linking, StyleProp, StyleSheet, TouchableOpacity, View, ViewStyle } from 'react-native';

import { contactStyles, contentStyles, eventStyles, layoutStyles, tileStyles } from '../profile/styles';
import { Chevron } from '../profile/chevron';
import { Avatar } from '../people/avatar';
import { AppState, getEmployees, getRequests } from '../reducers/app.reducer';
import { Department } from '../reducers/organization/department.model';

import { StyledText } from '../override/styled-text';
import { Employee, EmployeeId } from '../reducers/organization/employee.model';
import { ApplicationIcon } from '../override/application-icon';
import { calendarEventSetNewStatus, loadCalendarEvents } from '../reducers/calendar/calendar.action';
import { CalendarEvent, CalendarEventId, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { EmployeeDetailsEventsList } from './employee-details-events-list';
import { UserEmployeePermissions } from '../reducers/user/user-employee-permissions.model';
import { loadUserEmployeePermissions } from '../reducers/user/user.action';
import { HoursCreditCounter, VacationDaysCounter } from '../reducers/calendar/days-counters.model';
import { ConvertHoursCreditToDays } from '../reducers/calendar/convert-hours-credit-to-days';
import { Action, Dispatch } from 'redux';
import { openCompany, openDepartment, openRoom } from '../navigation/navigation.actions';
import { Nullable, Optional } from 'types';
import { IntervalTypeConverter } from '../reducers/calendar/interval-type-converter';
import { approve } from '../reducers/calendar/approval.action';
import { Approval } from '../reducers/calendar/approval.model';
import { EventActionContainer, EventActionProvider } from './event-action-provider';
import { capitalizeFirstLetter, uppercase } from '../utils/string';
import { endSearch } from '../reducers/search/search.action';
import { SearchType } from '../navigation/search/search-view';
import Style from '../layout/style';
import { equals } from '../utils/equitable';
import { EmployeesStore } from '../reducers/organization/employees.reducer';

//============================================================================
interface TileData {
    label: string;
    icon: string;
    style: ViewStyle;
    size: number;
    payload: Nullable<string>;
    onPress: Nullable<() => void>;
}

//============================================================================
interface EmployeeDetailsOwnProps {
    employee: Employee;
    department: Department;
    layoutStylesChevronPlaceholder: ViewStyle;
    showRequests?: boolean;
}

//============================================================================
interface EmployeeDetailsStateProps {
    events?: Map<EmployeeId, CalendarEvent[]>;
    requests?: Map<Employee, CalendarEvent[]>;
    employees?: EmployeesStore;
    approvals: Map<CalendarEventId, Set<Approval>>;
    hoursToIntervalTitle: (startWorkingHour: number, finishWorkingHour: number) => Nullable<string>;
    userEmployeePermissions: Nullable<Map<string, UserEmployeePermissions>>;
    userId: Nullable<EmployeeId>;
}

type EmployeeDetailsProps = EmployeeDetailsOwnProps & EmployeeDetailsStateProps;

//----------------------------------------------------------------------------
const mapStateToProps: MapStateToProps<EmployeeDetailsProps, EmployeeDetailsOwnProps, AppState> = (state: AppState, ownProps: EmployeeDetailsOwnProps): EmployeeDetailsProps => {

    const employees = getEmployees(state);

    return {
        employee: ownProps.employee,
        department: ownProps.department,
        layoutStylesChevronPlaceholder: ownProps.layoutStylesChevronPlaceholder,
        showRequests: ownProps.showRequests,
        events: state.calendar ? state.calendar.calendarEvents.events : undefined,
        employees: employees,
        requests: getRequests(state, employees),
        approvals: state.calendar ? state.calendar.calendarEvents.approvals : Map<CalendarEventId, Set<Approval>>(),
        hoursToIntervalTitle: state.calendar ? state.calendar.pendingRequests.hoursToIntervalTitle : IntervalTypeConverter.hoursToIntervalTitle,
        userEmployeePermissions: state.userInfo ? state.userInfo.permissions : null,
        userId: state.userInfo ? state.userInfo.employeeId : null,
    };
};

//============================================================================
interface EmployeeDetailsDispatchProps {
    loadCalendarEvents: (employeeId: string) => void;
    loadUserEmployeePermissions: (employeeId: string) => void;
    eventSetStatus: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
    eventApprove: (approverId: EmployeeId, employeeId: EmployeeId, calendarEvent: CalendarEvent) => void;
    openCompany: (departmentId: string) => void;
    openDepartment: (departmentId: string, departmentAbbreviation: string) => void;
    openRoom: (departmentId: string) => void;
}

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): EmployeeDetailsDispatchProps => ({
    loadCalendarEvents: (employeeId: string) => {
        dispatch(loadCalendarEvents(employeeId));
    },
    loadUserEmployeePermissions: (employeeId: string) => {
        dispatch(loadUserEmployeePermissions(employeeId));
    },
    eventSetStatus: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => {
        dispatch(calendarEventSetNewStatus(employeeId, calendarEvent, status));
    },
    eventApprove: (approverId: EmployeeId, employeeId: EmployeeId, calendarEvent: CalendarEvent) => {
        dispatch(approve(approverId, employeeId, calendarEvent.calendarEventId));
    },
    openCompany: (departmentId: string) => {
        dispatch(openCompany(departmentId));
        dispatch(endSearch(SearchType.People));
    },
    openDepartment: (departmentId: string, departmentAbbreviation: string) => {
        dispatch(openDepartment(departmentId, departmentAbbreviation));
        dispatch(endSearch(SearchType.People));
    },
    openRoom: (departmentId: string) => {
        dispatch(openRoom(departmentId));
        dispatch(endSearch(SearchType.People));
    }
});

//----------------------------------------------------------------------------
const TileSeparator = () => <View style={tileStyles.separator}/>;

//============================================================================
export class EmployeeDetailsImpl extends Component<EmployeeDetailsProps & EmployeeDetailsDispatchProps> {

    //----------------------------------------------------------------------------
    private permissionsAreDifferent(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps): boolean {
        const permissions = this.props.userEmployeePermissions;
        const nextPermissions = nextProps.userEmployeePermissions;

        if (!equals(permissions, nextPermissions)) {
            return true;
        }

        const employeeId = this.props.employee.employeeId;
        const nextEmployeeId = nextProps.employee.employeeId;

        const userPermissions = permissions ? permissions.get(employeeId, null) : null;
        const nextUserPermissions = nextPermissions ? nextPermissions.get(nextEmployeeId, null) : null;

        // noinspection RedundantIfStatementJS
        if (!equals(userPermissions, nextUserPermissions)) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    private eventsAreDifferent(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps): boolean {
        const events = this.props.events;
        const nextEvents = nextProps.events;

        const employeeId = this.props.employee.employeeId;
        const nextEmployeeId = nextProps.employee.employeeId;

        const employeeEvents = List.of(events ? events.get(employeeId, null) : null);
        const nextEmployeeEvents = List.of(nextEvents ? nextEvents.get(nextEmployeeId, null) : null);

        // noinspection RedundantIfStatementJS
        if (!equals(employeeEvents, nextEmployeeEvents)) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    private requestsAreDifferent(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps): boolean {
        const requests = this.props.requests;
        const nextRequests = nextProps.requests;

        // noinspection RedundantIfStatementJS
        if (!equals(requests, nextRequests)) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    private approvalsAreDifferent(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps): boolean {
        const approvals = this.props.approvals;
        const nextApprovals = nextProps.approvals;

        // noinspection RedundantIfStatementJS
        if (!equals(approvals, nextApprovals)) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps) {
        if (this.props.employee !== nextProps.employee) {
            return true;
        }

        if (this.permissionsAreDifferent(nextProps)) {
            return true;
        }

        if (this.eventsAreDifferent(nextProps)) {
            return true;
        }

        if (this.requestsAreDifferent(nextProps)) {
            return true;
        }

        // noinspection RedundantIfStatementJS
        if (this.approvalsAreDifferent(nextProps)) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    public componentDidMount() {
        this.props.loadCalendarEvents(this.props.employee.employeeId);
        this.props.loadUserEmployeePermissions(this.props.employee.employeeId);
    }

    //----------------------------------------------------------------------------
    public render() {
        const { employee, department, userEmployeePermissions, userId } = this.props;

        if (!userEmployeePermissions || !userId) {
            return null;
        }

        return (
            <View style={layoutStyles.container}>
                <View style={this.props.layoutStylesChevronPlaceholder}/>
                <View>
                    <Chevron/>
                    <View style={layoutStyles.avatarContainer}>
                        <Avatar photoUrl={employee.photoUrl} imageStyle={{ borderWidth: 0 }}
                                style={{ borderWidth: 3, backgroundColor: Style.color.base }}/>
                    </View>
                </View>
                <View style={layoutStyles.content}>

                    <StyledText style={contentStyles.name}>
                        {employee.name}
                    </StyledText>

                    <StyledText style={contentStyles.position}>
                        {uppercase(employee.position)}
                    </StyledText>

                    <TouchableOpacity onPress={this.openDepartment}>
                        <StyledText style={contentStyles.department}>
                            {uppercase(department.abbreviation)}
                        </StyledText>
                    </TouchableOpacity>

                    <View style={contentStyles.infoContainer}>
                        {this.getTiles(employee)}
                    </View>

                    <View style={contentStyles.contactsContainer}>
                        <View>
                            {
                                this.getContacts(employee)
                            }
                            {
                                this.renderDaysCounters(employee)
                            }
                        </View>
                    </View>

                    {
                        this.renderEventsAndRequests()
                    }

                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private tileStyle = (transparent: boolean): StyleProp<ViewStyle> => {
        if (transparent) {
            return [tileStyles.tile, { backgroundColor: 'transparent' }];
        } else {
            return tileStyles.tile;
        }
    };

    //----------------------------------------------------------------------------
    private getTiles(employee: Employee) {
        let roomNumber = employee && employee.roomNumber ? employee.roomNumber : '';
        let roomTitle: string = isNaN(Number(roomNumber)) ? roomNumber : `Room ${roomNumber}`;

        const tilesData: TileData[] = [
            {
                label: employee.birthDate.format('MMMM D'),
                icon: 'birthday',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 30,
                payload: null,
                onPress: null
            },
            {
                label: employee.hireDate.format('DD MMM YYYY'),
                icon: 'handshake',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 20,
                payload: null,
                onPress: null
            },
            {
                label: roomTitle,
                icon: 'office',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 25,
                payload: employee.roomNumber,
                onPress: this.openRoom
            },
            {
                label: 'Company',
                icon: 'org_structure',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 28,
                payload: employee.departmentId,
                onPress: this.openCompany
            }
        ];
        const lastIndex = tilesData.length - 1;

        return tilesData.map((tile, index) => (
            <React.Fragment key={tile.label}>
                <View style={tileStyles.container}>
                    {
                        tile.payload !== null ?
                            <TouchableOpacity onPress={tile.onPress ? tile.onPress : undefined}>
                                <View style={this.tileStyle(tile.onPress === null)}>
                                    <View style={tileStyles.iconContainer}>
                                        <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style}/>
                                    </View>
                                    <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                                </View></TouchableOpacity>
                            : <View style={this.tileStyle(tile.onPress === null)}>
                                <View style={tileStyles.iconContainer}>
                                    <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style}/>
                                </View>
                                <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                            </View>
                    }
                </View>
                {
                    lastIndex !== index ? <TileSeparator key={`${tile.label}-${index}`}/> : null
                }
            </React.Fragment>
        ));
    }

    //----------------------------------------------------------------------------
    private getContacts(employee: Employee) {
        const contactsData = [
            {
                icon: 'phone',
                text: employee.mobilePhone,
                title: 'Mobile Phone:',
                size: 35,
                prefix: 'tel:'
            },
            {
                icon: 'envelope',
                text: employee.email,
                title: 'Email:',
                size: 25,
                prefix: 'mailto:'
            }
        ];

        return contactsData.filter(c => c.text && c.text.length > 0).map((contact) => (
            <TouchableOpacity key={contact.title} onPress={this.openLink(`${contact.prefix}${contact.text}`)}>
                <View style={contactStyles.container}>
                    <View style={contactStyles.iconContainer}>
                        <ApplicationIcon name={contact.icon} size={contact.size} style={contactStyles.icon}/>
                    </View>
                    <View style={contactStyles.textContainer}>
                        <StyledText style={contactStyles.title}>{contact.title}</StyledText>
                        <StyledText style={contactStyles.text}>{contact.text}</StyledText>
                    </View>
                </View>
            </TouchableOpacity>
        ));
    }

    //----------------------------------------------------------------------------
    private renderDaysCounters(employee: Employee) {
        if (employee.vacationDaysLeft === null && employee.hoursCredit === null) {
            return null;
        }

        const { vacationDaysLeft, hoursCredit } = this.props.employee;

        const allVacationDaysCounter = new VacationDaysCounter(vacationDaysLeft);

        const daysConverter = new ConvertHoursCreditToDays();
        const calculatedDays = daysConverter.convert(hoursCredit);

        const hoursCreditCounter = new HoursCreditCounter(
            hoursCredit,
            calculatedDays.days ? calculatedDays.days : null,
            calculatedDays.rest ? calculatedDays.rest : null);
        const vacationTitle = 'vacation';
        const dayoffTitle = 'dayoff';

        return [
            <View key={vacationTitle} style={contactStyles.container}>
                <View style={contactStyles.iconContainer}>
                    <ApplicationIcon name={vacationTitle} size={35} style={contactStyles.icon}/>
                </View>
                <View style={contactStyles.textContainer}>
                    <StyledText style={contactStyles.title}>{'Days of vacation left:'}</StyledText>
                    <StyledText style={contactStyles.text}>{allVacationDaysCounter.toString()}</StyledText>
                </View>
            </View>,
            <View key={dayoffTitle} style={contactStyles.container}>
                <View style={contactStyles.iconContainer}>
                    <ApplicationIcon name={dayoffTitle} size={35} style={contactStyles.icon}/>
                </View>
                <View style={contactStyles.textContainer}>
                    <StyledText
                        style={contactStyles.title}>{capitalizeFirstLetter(hoursCreditCounter.title.join(' '))}</StyledText>
                    <StyledText style={contactStyles.text}>{hoursCreditCounter.toString()}</StyledText>
                </View>
            </View>
        ];
    }

    //----------------------------------------------------------------------------
    private renderEmployeeEvents(events: Optional<CalendarEvent[]>, userPermissions: Nullable<UserEmployeePermissions>) {
        if (!events || !events.length || !this.props.userId || !this.props.employee || !userPermissions) {
            return null;
        }

        const provider = new EventActionProvider(this.props.userId, this.props.eventSetStatus, this.props.eventApprove);
        const actions = provider.getEventActions(events, this.props.employee, userPermissions, this.props.approvals);

        return <View style={eventStyles.container}>
            <StyledText style={layoutStyles.header}>{capitalizeFirstLetter('Events')}</StyledText>
            <EmployeeDetailsEventsList
                eventActions={actions}
                hoursToIntervalTitle={this.props.hoursToIntervalTitle}
            />
        </View>;
    }

    //----------------------------------------------------------------------------
    private renderPendingRequests(requests: Optional<Map<Employee, CalendarEvent[]>>, userPermissions: Nullable<UserEmployeePermissions>) {
        if (!requests || !requests.size || !this.props.userId || !userPermissions) {
            return null;
        }

        const provider = new EventActionProvider(this.props.userId, this.props.eventSetStatus, this.props.eventApprove);

        const actions: EventActionContainer[] =
            requests.reduce<EventActionContainer[]>(
                (acc, events, employee) => acc.concat(provider.getRequestActions(events, employee, this.props.approvals)),
                []
            );

        return <View style={eventStyles.container}>
            <StyledText style={layoutStyles.header}>{capitalizeFirstLetter('Requests')}</StyledText>
            <EmployeeDetailsEventsList
                eventActions={actions}
                hoursToIntervalTitle={this.props.hoursToIntervalTitle}
                showUserAvatar={true}/>
        </View>;
    }

    //----------------------------------------------------------------------------
    private renderEventsAndRequests() {

        const { employee, userEmployeePermissions } = this.props;

        if (!userEmployeePermissions) {
            return null;
        }

        const permissions = userEmployeePermissions.get(employee.employeeId, null);

        const events = this.props.events ? this.props.events.get(employee.employeeId, undefined) : undefined;
        const requests = this.props.requests;

        const loadingEvents = !events;
        const loadingRequests = !requests;
        const showRequests = this.props.showRequests ? this.props.showRequests : false;

        if (loadingEvents || (showRequests && loadingRequests)) {
            return (
                <View style={eventStyles.loadingContainer}>
                    <ActivityIndicator color={Style.color.base}/>
                </View>
            );
        } else {
            return (
                <View>
                    {
                        this.renderPendingRequests(requests, permissions)
                    }
                    {
                        this.renderEmployeeEvents(events, permissions)
                    }
                </View>
            );
        }
    }

    //----------------------------------------------------------------------------
    private openLink(url: string) {
        return () => Linking.openURL(url).catch(err => console.error(err));
    }

    //----------------------------------------------------------------------------
    private openCompany = () => {
        return this.props.openCompany(this.props.employee.departmentId);
    };

    //----------------------------------------------------------------------------
    private openDepartment = () => {
        this.props.openDepartment(this.props.employee.departmentId, this.props.department.abbreviation);
    };

    //----------------------------------------------------------------------------
    private openRoom = () => {
        if (this.props.employee.roomNumber) {
            this.props.openRoom(this.props.employee.roomNumber);
        }
    };
}

export const EmployeeDetails = connect(mapStateToProps, mapDispatchToProps)(EmployeeDetailsImpl);
