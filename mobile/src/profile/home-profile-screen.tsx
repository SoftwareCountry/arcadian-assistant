import React, { Component } from 'react';
import { connect } from 'react-redux';
import { View, LayoutChangeEvent, Text, Image } from 'react-native';

import styles from '../layout/styles';
import { layoutStyles, contentStyles, tileStyles } from './styles';
import { Chevron } from './chevron';
import { Avatar } from '../people/avatar';
import { TopNavBar } from '../topNavBar/top-nav-bar';
import { AppState } from '../reducers/app.reducer';
import { UserInfoState } from '../reducers/user/user-info.reducer';
import { Department } from '../reducers/organization/department.model';

import * as icon from './icons';
import { StyledText } from '../override/styled-text';

const navBar = new TopNavBar('');

interface ProfileScreenProps {
    userInfo: UserInfoState;
    departments: Department[];
}

const mapStateToProps = (state: AppState): ProfileScreenProps => ({
    userInfo: state.userInfo,
    departments: state.organization.departments
});

class ProfileScreenImpl extends Component<ProfileScreenProps> {
    public static navigationOptions = navBar.configurate();

    public render() {
        const userInfo = this.props.userInfo;

        const employee = userInfo ? userInfo.employee : null;
        const name = employee ? employee.name : null;
        const position = this.uppercase(employee ? employee.position : null);

        const department = this.props.departments && employee ? this.props.departments.find((d) => d.departmentId === employee.departmentId) : null;

        const departmentAbbr = this.uppercase(department ? department.abbreviation : null);

        const base64 = userInfo ? (userInfo.employee ? userInfo.employee.photo.base64 : null) : null;
        const mime = userInfo ? (userInfo.employee ? userInfo.employee.photo.mimeType : null) : null;

        let tilesData: { label: string, icon: string }[] = [];
        if (employee) {
            tilesData.push({
                label: employee.birthDate.format('MMMM D'),
                icon: icon.cupcake
            });

            tilesData.push({
                label: employee.hireDate.format('YYYY-D-MM'),
                icon: icon.hire
            });

            tilesData.push({
                label: `Room ${employee.roomNumber}`,
                icon: icon.room
            });
        }

        if (department) {
            tilesData.push({
                label: 'Organization',
                icon: icon.organization
            });
        }

        const tiles = tilesData.map((tile) => (
            <View key={tile.label} style={tileStyles.container}>
                <View style={tileStyles.tile}>
                    <View style={tileStyles.iconContainer}>
                        <Image source={{ uri: tile.icon }} style={tileStyles.icon} resizeMode='contain' />
                    </View>
                    <StyledText style={tileStyles.text}>{tile.label}</StyledText>
                </View>
            </View>
        ));

        return <View style={styles.container}>
            <View style={layoutStyles.container}>
                <View style={layoutStyles.chevronPlaceholder}></View>
                <View>
                    <Chevron />
                    <View style={layoutStyles.avatarContainer}>
                        <Avatar mimeType={mime} photoBase64={base64} />
                    </View>
                </View>

                <View style={layoutStyles.content}>
                    <StyledText style={contentStyles.name}>
                        {name}
                    </StyledText>
                    <StyledText style={contentStyles.position}>
                        {position}
                    </StyledText>
                    <StyledText style={contentStyles.department}>
                        {departmentAbbr}
                    </StyledText>

                    <View style={contentStyles.infoContainer}>
                        {tiles}
                    </View>

                    <View style={contentStyles.contactsContainer}>

                    </View>
                </View>

            </View>
        </View>;
    }

    private uppercase(text: string) {
        return text ? text.toUpperCase() : text;
    }
}

export const HomeProfileScreen = connect(mapStateToProps)(ProfileScreenImpl);
