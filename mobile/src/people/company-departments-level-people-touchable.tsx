import React, { Component } from 'react';
import { TouchableOpacity, Animated, Easing, StyleSheet } from 'react-native';
import { companyDepartmentLevelPeople } from './styles';
import { Employee } from '../reducers/organization/employee.model';

interface CompanyDepartmentsLevelPeopleTouchableProps {
    employee: Employee;
    onPress: (employee: Employee) => void;
}

export class CompanyDepartmentsLevelPeopleTouchable extends Component<CompanyDepartmentsLevelPeopleTouchableProps> {
    private readonly animatedOpacity = new Animated.Value(0);

    public componentDidMount() {
        Animated.timing(this.animatedOpacity, {
            toValue: 1,
            duration: 600,
            easing: Easing.linear,
            useNativeDriver: true
        }).start();
    }

    public render() {
        const listItemStyles = StyleSheet.flatten([
            companyDepartmentLevelPeople.listItem,
            {
                opacity: this.animatedOpacity as any
            }
        ]);

        return (
            <TouchableOpacity onPress={this.onPress}>
                <Animated.View style={listItemStyles}>
                    {
                        this.props.children
                    }
                </Animated.View>
            </TouchableOpacity>
        );
    }

    private onPress = () => {
        this.props.onPress(this.props.employee);
    };
}
