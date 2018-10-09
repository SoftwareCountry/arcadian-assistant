import React, { Component } from 'react';
import { View, LayoutChangeEvent } from 'react-native';
import { companyDepartments } from './styles';

interface LevelNodesContainerProps {
    children: (width: number, height: number) => JSX.Element;
}

interface LevelNodesContainerState {
    width: number;
    height: number;
}

export class CompanyDepartmentsLevelNodesContainer extends Component<LevelNodesContainerProps, LevelNodesContainerState>  {
    constructor(props: LevelNodesContainerProps) {
        super(props);
        this.state = {
            width: 0,
            height: 0
        };
    }

    public render() {
        const children = this.state.height && this.state.width
            ? this.props.children(this.state.width, this.state.height)
            : null;

        return (
            <View style={companyDepartments.nodesContainer} onLayout={this.onLayoutContainer}>
                {
                    children
                }
            </View>
        );
    }

    private onLayoutContainer = (e: LayoutChangeEvent) => {
        this.setState({
            width: e.nativeEvent.layout.width,
            height: e.nativeEvent.layout.height
        });
    }
}