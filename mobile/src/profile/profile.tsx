import React, { Component } from 'react';
import { connect } from 'react-redux';
import { View, LayoutChangeEvent, Text, Image, ImageStyle, StyleSheet } from 'react-native';

import styles from '../layout/styles';
import { layoutStyles, contentStyles, tileStyles, contactStyles } from './styles';
import { Chevron } from './chevron';
import { Avatar } from '../people/avatar';
import { TopNavBar } from '../topNavBar/top-nav-bar';
import { AppState } from '../reducers/app.reducer';
import { UserInfoState } from '../reducers/user/user-info.reducer';
import { Department } from '../reducers/organization/department.model';

import { StyledText } from '../override/styled-text';
import { Employee } from '../reducers/organization/employee.model';
import { ApplicationIcon } from '../override/application-icon';

interface ProfileProps {
    employee: Employee;
    department: Department;
}

export class Profile extends Component<ProfileProps> {
    public render() {
        const { employee, department } = this.props;

        if (!employee || !department) {
            return null;
        }

        const tiles = this.getTiles(employee);
        const contacts = this.getContacts(employee);

        return <View style={styles.container}>
            <View style={layoutStyles.container}>
                <View style={layoutStyles.chevronPlaceholder}></View>
                <View>
                    <Chevron />
                    <View style={layoutStyles.avatarContainer}>
                        <Avatar photo={employee.photo} imageStyle={{borderWidth: 0}} style={{borderWidth: 3}} />
                    </View>
                </View>

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
                </View>

            </View>
        </View>;
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
                size: 30
            },
            {
                label: employee.hireDate.format('YYYY-D-MM'),
                icon: 'handshake',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 20
            },
            {
                label: `Room ${employee.roomNumber}`,
                icon: 'office',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 25
            },
            {
                label: 'Organization',
                icon: 'org_structure',
                style: StyleSheet.flatten([tileStyles.icon]),
                size: 28
            }
        ];

        return tilesData.map((tile) => (
            <View key={tile.label} style={tileStyles.container}>
                <View style={tileStyles.tile}>
                    <View style={tileStyles.iconContainer}>
                        <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style} />
                    </View>
                    <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                </View>
            </View>
        ));
    }

    private getContacts(employee: Employee) {
        const contactsData = [
            {
                icon: 'phone',
                text: employee.mobilePhone,
                title: 'Mobile Phone:',
                size: 30
            },
            {
                icon: 'envelope',
                text: employee.email,
                title: 'Email:',
                size: 30
            }
        ];

        return contactsData.map((contact) => (
            <View style={contactStyles.container} key={contact.title}>
                <View style={contactStyles.iconContainer}>
                    <ApplicationIcon name={contact.icon} size={contact.size} style={contactStyles.icon}/>
                </View>
                <View style={contactStyles.textContainer}>
                    <StyledText style={contactStyles.title}>{contact.title}</StyledText>
                    <StyledText style={contactStyles.text}>{contact.text}</StyledText>
                </View>
            </View>
        ));
    }
}