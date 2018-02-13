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
                        <Avatar photo={employee.photo} />
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
        const tilesData: { label: string, icon: any, style: ImageStyle }[] = [
            {
                label: employee.birthDate.format('MMMM D'),
                icon: require('../../src/profile/icons/birthDate.png'),
                style: StyleSheet.flatten([tileStyles.icon, tileStyles.iconBirthDay])
            },
            {
                label: employee.hireDate.format('YYYY-D-MM'),
                icon: require('../../src/profile/icons/hireDate.png'),
                style: StyleSheet.flatten([tileStyles.icon, tileStyles.iconHireDate])
            },
            {
                label: `Room ${employee.roomNumber}`,
                icon: require('../../src/profile/icons/room.png'),
                style: StyleSheet.flatten([tileStyles.icon, tileStyles.iconRoom])
            },
            {
                label: 'Organization',
                icon: require('../../src/profile/icons/organization.png'),
                style: StyleSheet.flatten([tileStyles.icon, tileStyles.iconOrganization])
            }
        ];

        return tilesData.map((tile) => (
            <View key={tile.label} style={tileStyles.container}>
                <View style={tileStyles.tile}>
                    <View style={tileStyles.iconContainer}>
                        <Image source={tile.icon} style={tile.style} resizeMode='contain' />
                    </View>
                    <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                </View>
            </View>
        ));
    }

    private getContacts(employee: Employee) {
        const contactsData: { icon: any, text: string, title: string }[] = [
            {
                icon: require('../../src/profile/icons/phone.png'),
                text: employee.mobilePhone,
                title: 'Mobile Phone:'
            },
            {
                icon: require('../../src/profile/icons/email.png'),
                text: employee.email,
                title: 'Email:'
            }
        ];

        return contactsData.map((contact) => (
            <View style={contactStyles.container} key={contact.title}>
                <View style={contactStyles.iconContainer}>
                    <Image source={contact.icon} style={contactStyles.icon} resizeMode='center' />
                </View>
                <View style={contactStyles.textContainer}>
                    <StyledText style={contactStyles.title}>{contact.title}</StyledText>
                    <StyledText style={contactStyles.text}>{contact.text}</StyledText>
                </View>
            </View>
        ));
    }
}