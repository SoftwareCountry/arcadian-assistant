import React, { Component } from 'react';
import { connect, Dispatch } from 'react-redux';
import { Map } from 'immutable';
import moment, { Moment } from 'moment';
import { View, LayoutChangeEvent, Text, Image, ImageStyle, StyleSheet, ScrollView, Linking, TouchableOpacity, ViewStyle, Dimensions } from 'react-native';

import { layoutStyles, contentStyles, tileStyles, contactStyles } from '../profile/styles';
import { Chevron } from '../profile/chevron';
import { Avatar } from '../people/avatar';
import { TopNavBar } from '../navigation/top-nav-bar';
import { AppState } from '../reducers/app.reducer';
import { UserInfoState } from '../reducers/user/user-info.reducer';
import { Department } from '../reducers/organization/department.model';

import { StyledText } from '../override/styled-text';
import { Employee } from '../reducers/organization/employee.model';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { openCompanyAction } from './employee-details-dispatcher';
import { loadCalendarEvents } from '../reducers/calendar/calendar.action';
import { CalendarEvent } from '../reducers/calendar/calendar-event.model';
import { eventDialogTextDateFormat } from '../calendar/event-dialog/event-dialog-base';
import { EmployeeDetailsEventsList } from './employee-details-events-list';

interface EmployeeDetailsProps {
    employee?: Employee;
    department: Department;
    layoutStylesChevronPlaceholder?: ViewStyle;
    events?: Map<string, CalendarEvent[]>;
    eventsPredicate?: (event: CalendarEvent) => boolean;
}

const mapStateToProps = (state: AppState, props: EmployeeDetailsProps): EmployeeDetailsProps => ({
    department: props.department,
    events: state.calendar.calendarEvents.events,
    eventsPredicate: (event: CalendarEvent) => {
        const now = moment().local(true);
        return event.dates.endDate.isSameOrAfter(now);
    }
});

const TileSeparator = () => <View style = {tileStyles.separator}></View>;

interface EmployeeDetailsDispatchProps {
    onCompanyClicked: (departmentId: string) => void;
    loadCalendarEvents: (employeeId: string) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeeDetailsDispatchProps => ({
    onCompanyClicked: (departmentId: string) => dispatch( openCompanyAction(departmentId)),
    loadCalendarEvents: (employeeId: string) => dispatch(loadCalendarEvents(employeeId))
});

export class EmployeeDetailsImpl extends Component<EmployeeDetailsProps & EmployeeDetailsDispatchProps> {
    public componentDidMount() {
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
                            (events !== undefined && events.length > 0) ? 
                            <EmployeeDetailsEventsList events={events} /> : null
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

    private openLink(url: string) {
        return () => Linking.openURL(url).catch(err => console.error(err));
    }

    private openCompany = () => {
        return this.props.onCompanyClicked(this.props.employee.departmentId);
    }
}

export const EmployeeDetails = connect(mapStateToProps, mapDispatchToProps)(EmployeeDetailsImpl);