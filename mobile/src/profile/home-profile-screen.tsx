import React, { Component } from 'react';
import { connect } from 'react-redux';
import { View, LayoutChangeEvent, Text, Image } from 'react-native';

import styles from '../layout/styles';
import { layoutStyles, contentStyles } from './styles';
import { Chevron } from './chevron';
import { Avatar } from '../people/avatar';
import { TopNavBar } from '../topNavBar/top-nav-bar';
import { AppState } from '../reducers/app.reducer';
import { UserInfoState } from '../reducers/user/user-info.reducer';
import { Department } from '../reducers/organization/department.model';

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
        //console.log('User ', userInfo);
        const employee = userInfo ? userInfo.employee : null;
        const name = employee ? employee.name : '';
        const position = this.uppercase(employee ? employee.position : '');

        const department = this.props.departments && employee ? this.props.departments.find((d) => d.departmentId === employee.departmentId) : null;

        const departmentAbbr = this.uppercase(department ? department.abbreviation : '');

        const base64 = userInfo ? (userInfo.employee ? userInfo.employee.photo.base64 : null) : null;
        const mime = userInfo ? (userInfo.employee ? userInfo.employee.photo.mimeType : null) : null;

        return <View style={styles.container}>
            <View style={layoutStyles.container}>
                <View style={layoutStyles.chevronPlaceholder}></View>
                <View>
                    <Chevron/>
                    <View style={layoutStyles.avatarContainer}>
                        <Avatar mimeType={mime} photoBase64={base64} />
                    </View>
                </View>

                <View style={layoutStyles.content}>
                    <Text style={contentStyles.name}>
                        {name}
                    </Text>
                    <Text style={contentStyles.position}>
                        {position}
                    </Text>
                    <Text style={contentStyles.department}>
                        {departmentAbbr}
                    </Text>

                    <View style={contentStyles.infoContainer}>
                    </View>

                    <View style={contentStyles.contactsContainer}>

                    </View>
                </View>

            </View>
        </View>;
    }

    private uppercase(text: string) {
        return text ? text.toUpperCase() : '';
    }
}

export const HomeProfileScreen = connect(mapStateToProps)(ProfileScreenImpl);
