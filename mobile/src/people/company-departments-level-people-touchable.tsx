import React, { Component } from 'react';
import { TouchableOpacity } from 'react-native';
import { companyDepartmentLevelPeople } from './styles';

interface CompanyDepartmentsLevelPeopleTouchableProps {
    employeeId: string;
    onPress: (employeeId: string) => void;
}

export class CompanyDepartmentsLevelPeopleTouchable extends Component<CompanyDepartmentsLevelPeopleTouchableProps> {
    public render() {
        return (
            <TouchableOpacity 
                style={companyDepartmentLevelPeople.listItem} 
                onPress={this.onPress}>
                    {
                        this.props.children
                    }
            </TouchableOpacity>
        );
    }

    private onPress = () => {
        this.props.onPress(this.props.employeeId)
    }
}