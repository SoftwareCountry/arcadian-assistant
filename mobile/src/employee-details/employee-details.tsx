import React, { Component } from 'react';
import { connect, Dispatch, MapStateToProps } from 'react-redux';
import { Map } from 'immutable';
import {View, StyleSheet, ScrollView, Linking, TouchableOpacity, ViewStyle, StyleProp} from 'react-native';

import { layoutStyles, contentStyles, tileStyles, contactStyles } from '../profile/styles';
import { Chevron } from '../profile/chevron';
import { Avatar } from '../people/avatar';
import { AppState } from '../reducers/app.reducer';
import { Department } from '../reducers/organization/department.model';

import { StyledText } from '../override/styled-text';
import { Employee } from '../reducers/organization/employee.model';
import { ApplicationIcon } from '../override/application-icon';
import { openCompanyAction, openDepartmentAction, openRoomAction } from './employee-details-dispatcher';
import { loadCalendarEvents, calendarEventSetNewStatus } from '../reducers/calendar/calendar.action';
import { CalendarEvent, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { EmployeeDetailsEventsList } from './employee-details-events-list';
import { UserEmployeePermissions } from '../reducers/user/user-employee-permissions.model';
import { loadUserEmployeePermissions } from '../reducers/user/user.action';
import {DaysCounters} from '../calendar/days-counters/days-counters';
import { daysCountersStyles } from '../calendar/days-counters/styles';
import { DaysCounter, EmptyDaysCounter } from '../calendar/days-counters/days-counter';
import { DaysCountersModel, HoursCreditCounter, VacationDaysCounter } from '../reducers/calendar/days-counters.model';
import { ConvertHoursCreditToDays } from '../reducers/calendar/convert-hours-credit-to-days';

interface TileData {
    label: string;
    icon: string;
    style: ViewStyle;
    size: number;
    payload: string;
    onPress: () => void;
}

interface EmployeeDetailsOwnProps {
    employee: Employee;
    department: Department;
    layoutStylesChevronPlaceholder: ViewStyle;
    requests?: Map<Employee, CalendarEvent[]>;
}

interface EmployeeDetailsStateProps {
    events: Map<string, CalendarEvent[]>;
    hoursToIntervalTitle: (startWorkingHour: number, finishWorkingHour: number) => string;
    userEmployeePermissions: Map<string, UserEmployeePermissions>;
}

type EmployeeDetailsProps = EmployeeDetailsOwnProps & EmployeeDetailsStateProps;

const mapStateToProps: MapStateToProps<EmployeeDetailsProps, EmployeeDetailsOwnProps, AppState> = (state: AppState, ownProps: EmployeeDetailsOwnProps): EmployeeDetailsProps => ({
    employee: ownProps.employee,
    department: ownProps.department,
    layoutStylesChevronPlaceholder: ownProps.layoutStylesChevronPlaceholder,
    events: state.calendar.calendarEvents.events,
    requests: ownProps.requests,
    hoursToIntervalTitle: state.calendar.pendingRequests.hoursToIntervalTitle,
    userEmployeePermissions: state.userInfo.permissions
});

const TileSeparator = () => <View style = {tileStyles.separator}></View>;

interface EmployeeDetailsDispatchProps {
    onCompanyClicked: (departmentId: string) => void;
    loadCalendarEvents: (employeeId: string) => void;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
    loadUserEmployeePermissions: (employeeId: string) => void;
    openDepartment: (departmentId: string) => void;
    openRoom: (departmentId: string) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeeDetailsDispatchProps => ({
    onCompanyClicked: (departmentId: string) => dispatch( openCompanyAction(departmentId)),
    loadCalendarEvents: (employeeId: string) => dispatch(loadCalendarEvents(employeeId)),
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => dispatch(calendarEventSetNewStatus(employeeId, calendarEvent, status)),
    loadUserEmployeePermissions: (employeeId: string) => { dispatch(loadUserEmployeePermissions(employeeId)); },
    openDepartment: (departmentId: string) => { dispatch(openDepartmentAction(departmentId)); },
    openRoom: (departmentId: string) => { dispatch(openRoomAction(departmentId)); }
});

export class EmployeeDetailsImpl extends Component<EmployeeDetailsProps & EmployeeDetailsDispatchProps> {
    public shouldComponentUpdate(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps) {
        const requests = this.props.requests;
        const nextRequests = nextProps.requests;
        const events = this.props.events;
        const nextEvents = nextProps.events;

        if (this.props.employee !== nextProps.employee) {
            return true;
        }

        if (!this.props.userEmployeePermissions.equals(nextProps.userEmployeePermissions)) {

            const permissions = this.props.userEmployeePermissions.get(this.props.employee.employeeId);
            const nextPermissions = nextProps.userEmployeePermissions.get(nextProps.employee.employeeId);

            return !permissions || !permissions.equals(nextPermissions);
        }

        if (!events.equals(nextEvents)) {
            const calendarEvents = events.get(this.props.employee.employeeId);
            const nextCalendarEvents = nextEvents.get(nextProps.employee.employeeId);

            return calendarEvents !== nextCalendarEvents;
        }

        return false;
    }

    public componentDidMount() {
        this.props.loadCalendarEvents(this.props.employee.employeeId);
        this.props.loadUserEmployeePermissions(this.props.employee.employeeId);
    }

    public render() {
        const { employee, department, userEmployeePermissions } = this.props;

        if (!employee || !department) {
            return null;
        }

        const permissions = userEmployeePermissions.get(employee.employeeId);

        const tiles = this.getTiles(employee);
        const contacts = this.getContacts(employee);

        let events = this.props.events.get(employee.employeeId);

        return (
                <View style={layoutStyles.container}>
                    <View style={this.props.layoutStylesChevronPlaceholder}></View>
                    <View>
                        <Chevron />
                        <View style={layoutStyles.avatarContainer}>
                            <Avatar photoUrl={employee.photoUrl} imageStyle={{ borderWidth: 0 }} style={{ borderWidth: 3 }} />
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
                        <TouchableOpacity onPress={this.openDepartment}>
                            <StyledText style={contentStyles.department}>
                                {this.uppercase(department.abbreviation)}
                            </StyledText>
                        </TouchableOpacity>

                        <View style={contentStyles.infoContainer}>
                            {tiles}
                        </View>

                        <View style={contentStyles.contactsContainer}>
                            <View>
                                {contacts}
                            </View>
                        </View>

                        {
                            this.renderDaysCounters(employee)
                        }
                        {
                            this.renderPendingRequests()
                        }
                        {
                            this.renderEmployeeEvents(events, permissions)
                        }

                    </View>
                    </ScrollView>
                </View>
        );
    }

    private renderDaysCounters = (employee: Employee) => {
        const allVacationDaysCounter = new VacationDaysCounter(employee.vacationDaysLeft);

        const daysConverter = new ConvertHoursCreditToDays();
        const calculatedDays = daysConverter.convert(employee.hoursCredit);

        const hoursCreditCounter = new HoursCreditCounter(employee.hoursCredit, calculatedDays.days, calculatedDays.rest);

        const counters: DaysCountersModel = {
            allVacationDays: allVacationDaysCounter,
            hoursCredit: hoursCreditCounter,
        };

        if (employee.vacationDaysLeft !== null || employee.hoursCredit !== null) {
            return <DaysCounters explicitCounters={counters}
                                 explicitStyle={{ marginBottom: 30, marginTop: -30, }}/>;
        } else {
            return null;
        }
    };

    private uppercase(text: string) {
        return text ? text.toUpperCase() : text;
    }

    private tileStyle = (transparent: boolean): StyleProp<ViewStyle> => {
        if (transparent) {
            return [tileStyles.tile, {backgroundColor: 'transparent'}];
        } else {
            return tileStyles.tile;
        }
    };

    private getTiles(employee: Employee) {
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
                label: employee.hireDate.format('YYYY-D-MM'),
                icon: 'handshake',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 20,
                payload: null,
                onPress: null
            },
            {
                label: `Room ${employee.roomNumber}`,
                icon: 'office',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 25,
                payload: employee.roomNumber,
                onPress: this.openRoom
            },
            {
                label: 'Organization',
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
                    <TouchableOpacity onPress={tile.onPress}>
                    <View style={this.tileStyle(tile.onPress === null)}>
                        <View style={tileStyles.iconContainer}>
                            <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style} />
                        </View>
                        <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                    </View></TouchableOpacity>
                : <View style={this.tileStyle(tile.onPress === null)}>
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

    private renderEmployeeEvents(events: CalendarEvent[], userPermissions: UserEmployeePermissions) {
        if (!events || !events.length || !this.props.employee) {
            return null;
        }

        const employeeToCalendarEvents = Map<Employee, CalendarEvent[]>([ [ this.props.employee, events ] ]);
        const canApprove = userPermissions ? userPermissions.canApproveCalendarEvents : false;
        const canReject = userPermissions ? userPermissions.canRejectCalendarEvents : false;

        return <View>
            <StyledText style={layoutStyles.header}>EVENTS</StyledText>
            <EmployeeDetailsEventsList
                events={employeeToCalendarEvents}
                eventSetNewStatusAction={this.props.eventSetNewStatusAction}
                hoursToIntervalTitle={this.props.hoursToIntervalTitle}
                canApprove={canApprove}
                canReject={canReject}
            />
        </View>;
    }

    private renderPendingRequests() {
        if (!this.props.requests || !this.props.requests.size) {
            return null;
        }

        return <React.Fragment>
            <StyledText style={layoutStyles.header}>REQUESTS</StyledText>
            <EmployeeDetailsEventsList
                events={this.props.requests}
                eventSetNewStatusAction={this.props.eventSetNewStatusAction}
                hoursToIntervalTitle={this.props.hoursToIntervalTitle}
                showUserAvatar={true}
                canApprove={true}
                canReject={true} />
        </React.Fragment>;
    }

    private openLink(url: string) {
        return () => Linking.openURL(url).catch(err => console.error(err));
    }

    private openCompany = () => {
        return this.props.onCompanyClicked(this.props.employee.departmentId);
    };

    private openDepartment = () => {
        this.props.openDepartment(this.props.employee.departmentId);
    };

    private openRoom = () => {
        if (this.props.employee.roomNumber) {
            this.props.openRoom(this.props.employee.roomNumber);
        }
    };
}

export const EmployeeDetails = connect(mapStateToProps, mapDispatchToProps)(EmployeeDetailsImpl);
