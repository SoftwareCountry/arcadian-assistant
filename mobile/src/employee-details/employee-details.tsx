import React, { Component } from 'react';
import { connect, Dispatch } from 'react-redux';
import { Map } from 'immutable';
import { View, StyleSheet, ScrollView, Linking, TouchableOpacity, ViewStyle } from 'react-native';

import { layoutStyles, contentStyles, tileStyles, contactStyles } from '../profile/styles';
import { Chevron } from '../profile/chevron';
import { Avatar } from '../people/avatar';
import { AppState } from '../reducers/app.reducer';
import { Department } from '../reducers/organization/department.model';

import { StyledText } from '../override/styled-text';
import { Employee } from '../reducers/organization/employee.model';
import { ApplicationIcon } from '../override/application-icon';
import { openCompanyAction } from './employee-details-dispatcher';
import { loadCalendarEvents, calendarEventSetNewStatus, loadPendingRequests } from '../reducers/calendar/calendar.action';
import { CalendarEvent, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { EmployeeDetailsEventsList } from './employee-details-events-list';
import { EmployeesStore } from '../reducers/organization/employees.reducer';

interface EmployeeDetailsProps {
    employee?: Employee;
    employees?: EmployeesStore;
    department: Department;
    layoutStylesChevronPlaceholder?: ViewStyle;
    events?: Map<string, CalendarEvent[]>;
    eventsPredicate?: (event: CalendarEvent) => boolean;
    requests?: Map<string, CalendarEvent[]>;
    hoursToIntervalTitle?: (startWorkingHour: number, finishWorkingHour: number) => string;
    showPendingRequests?: Boolean;
}

const mapStateToProps = (state: AppState, props: EmployeeDetailsProps): EmployeeDetailsProps => ({
    department: props.department,
    employees: state.organization.employees,
    events: state.calendar.calendarEvents.events,
    eventsPredicate: state.calendar.calendarEvents.eventsPredicate,
    requests: state.calendar.calendarEvents.requests,
    hoursToIntervalTitle: state.calendar.calendarEvents.hoursToIntervalTitle
});

const TileSeparator = () => <View style = {tileStyles.separator}></View>;

interface EmployeeDetailsDispatchProps {
    onCompanyClicked: (departmentId: string) => void;
    loadCalendarEvents: (employeeId: string) => void;
    loadPendingRequests: () => void;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeeDetailsDispatchProps => ({
    onCompanyClicked: (departmentId: string) => dispatch( openCompanyAction(departmentId)),
    loadCalendarEvents: (employeeId: string) => dispatch(loadCalendarEvents(employeeId)),
    loadPendingRequests: () => dispatch(loadPendingRequests()),
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => dispatch(calendarEventSetNewStatus(employeeId, calendarEvent, status))
});

export class EmployeeDetailsImpl extends Component<EmployeeDetailsProps & EmployeeDetailsDispatchProps> {
    public shouldComponentUpdate(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps) {
        const employees = this.props.employees.employeesById;
        const nextEmployees = nextProps.employees.employeesById;
        const requests = this.props.requests;
        const nextRequests = nextProps.requests;
        const events = this.props.events;
        const nextEvents = nextProps.events;

        if (!requests.equals(nextRequests) || !events.equals(nextEvents)) {
            return true;
        }

        if (!employees.equals(nextEmployees)) {
            let newEmployeesSubset = nextEmployees.filter(employee => {
                return !employees.has(employee.employeeId);
            });

            if (requests.keySeq().some(key => newEmployeesSubset.has(key))) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }
    
    public componentDidMount() {
        this.props.loadPendingRequests();
        this.props.loadCalendarEvents(this.props.employee.employeeId);
    }
    public render() {
        const { employee, department } = this.props;

        if (!employee || !department) {
            return null;
        }

        const tiles = this.getTiles(employee);
        const contacts = this.getContacts(employee);

        let events = this.props.events.get(employee.employeeId);

        if (events !== undefined) {
            events = events.filter(this.props.eventsPredicate);
        }

        const requests =  this.props.showPendingRequests ? this.props.requests : undefined;

        return (
                <View style={layoutStyles.container}>
                    <View style={this.props.layoutStylesChevronPlaceholder}></View>
                    <View>
                        <Chevron />
                        <View style={layoutStyles.avatarContainer}>
                            <Avatar photo={employee.photo} imageStyle={{ borderWidth: 0 }} style={{ borderWidth: 3 }} />
                        </View>
                    </View>
                    <ScrollView style={layoutStyles.scrollView} alwaysBounceVertical = {false}>
                    <View style={layoutStyles.content}>
                        <StyledText style={contentStyles.name}>
                            {employee.name}
                        </StyledText>
                        <StyledText style={contentStyles.position}>
                            {this.uppercase(employee.position)}
                        </StyledText>
                        <StyledText style={contentStyles.department}>
                            {this.uppercase(department.abbreviation)}
                        </StyledText>

                        <View style={contentStyles.infoContainer}>
                            {tiles}
                        </View>

                        <View style={contentStyles.contactsContainer}>
                            <View>
                                {contacts}
                            </View>
                        </View>

                        {
                            (requests !== undefined && requests.size > 0) ? this.getPendingRequests(requests) : null
                        }

                        {
                            (events !== undefined && events.length > 0) ? 
                            <View>
                                <StyledText style={layoutStyles.header}>EVENTS</StyledText>
                                <EmployeeDetailsEventsList 
                                    events={events} 
                                    employee={employee}
                                    eventSetNewStatusAction={this.props.eventSetNewStatusAction}
                                    hoursToIntervalTitle={this.props.hoursToIntervalTitle}
                                    eventManagementEnabled={(this.props.requests.has(employee.employeeId))} 
                                />
                            </View> : null
                        }

                    </View>
                    </ScrollView>
                </View>
        );
    }

    private uppercase(text: string) {
        return text ? text.toUpperCase() : text;
    }

    private getTiles(employee: Employee) {
        const tilesData = [
            {
                label: employee.birthDate.format('MMMM D'),
                icon: 'birthday',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 30,
                payload: null
            },
            {
                label: employee.hireDate.format('YYYY-D-MM'),
                icon: 'handshake',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 20,
                payload: null
            },
            {
                label: `Room ${employee.roomNumber}`,
                icon: 'office',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 25,
                payload: null
            },
            {
                label: 'Organization',
                icon: 'org_structure',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 28,
                payload: employee.departmentId
            }
        ];
        const lastIndex = tilesData.length - 1;

        return tilesData.map((tile, index) => (
            <React.Fragment key={tile.label}>
            <View style={tileStyles.container}>
            {
                tile.payload !== null ?
                    <TouchableOpacity onPress={this.openCompany}>
                    <View style={tileStyles.tile}>
                        <View style={tileStyles.iconContainer}>
                            <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style} />
                        </View>
                        <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                    </View></TouchableOpacity>
                : <View style={tileStyles.tile}>
                    <View style={tileStyles.iconContainer}>
                        <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style} />
                    </View>
                    <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                </View>
            }
            </View>
            {
                lastIndex !== index ? <TileSeparator key = {`${tile.label}-${index}`} /> : null
            }
            </React.Fragment>
        ));
    }

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
                    <View style={contactStyles.iconContainer} >
                        <ApplicationIcon name={contact.icon} size={contact.size} style={contactStyles.icon} />
                    </View>
                    <View style={contactStyles.textContainer}>
                        <StyledText style={contactStyles.title}>{contact.title}</StyledText>
                        <StyledText style={contactStyles.text}>{contact.text}</StyledText>
                    </View>
                </View>
            </TouchableOpacity>
        ));
    }

    private getPendingRequests(requests: Map<string, CalendarEvent[]>) {
        return <React.Fragment>
            <StyledText style={layoutStyles.header}>REQUESTS</StyledText>
            {
                requests.keySeq().map((key) => (
                    <EmployeeDetailsEventsList
                        key={key}
                        events={requests.get(key)} 
                        employee={this.props.employees.employeesById.get(key)}
                        eventSetNewStatusAction={this.props.eventSetNewStatusAction}
                        hoursToIntervalTitle={this.props.hoursToIntervalTitle}
                        showUserAvatar
                        pendingRequestMode
                        eventManagementEnabled />
                ))
            }
        </React.Fragment>;
    }

    private openLink(url: string) {
        return () => Linking.openURL(url).catch(err => console.error(err));
    }

    private openCompany = () => {
        return this.props.onCompanyClicked(this.props.employee.departmentId);
    }
}

export const EmployeeDetails = connect(mapStateToProps, mapDispatchToProps)(EmployeeDetailsImpl);